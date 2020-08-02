using System;
using System.Collections.Generic;
using System.Text;
using ToDo.Domain.Entities;
using ToDo.Domain.Interfaces;
using ToDo.API.Helpers;
using Microsoft.Extensions.Options;
using ToDo.Infrastructure.Data;
using System.Linq;
using ToDo.Domain.Helpers;
using MailKit;
using System.Security.Cryptography;
using System.Reflection.Emit;
using ToDo.Domain.Models;

namespace ToDo.Infrastructure.Services
{
    public class UserService : IUserRepository
    {
        private ToDoContext _context;
        private readonly IEmailService _emailService;
        public UserService(ToDoContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }
        public User Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;
            var user = _context.Users.SingleOrDefault(x => x.UserName == username);

            if (user == null)
                return null;

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            return user;
        }

        public IEnumerable<User> GetAll()
        {
            return _context.Users;
        }

        public User GetById(int id)
        {
            return _context.Users.Find(id);
        }

        public User Create(User user, string password, string origin)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("Password is required");
            if(_context.Users.Any(x => x.UserName == user.UserName))
                throw new AppException("Username \"" + user.UserName + "\" is already taken");

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            if (string.IsNullOrWhiteSpace(user.Role)) user.Role = Role.User;

            user.Created = DateTime.Now;
            user.VerificationToken = randomTokenString();

            _context.Users.Add(user);
            _context.SaveChanges();

            //send email
            sendVerificationEmail(user, origin);

            return user;
        }

        public void VerifyEmail(string token)
        {
            var user = _context.Users.SingleOrDefault(x => x.VerificationToken == token);

            if (user == null) throw new AppException("Verification failed");

            user.IsVerified = true;

            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public void ForgotPassword(ForgotPasswordRequestModel model, string origin)
        {
            var user = _context.Users.SingleOrDefault(x => x.Email == model.Email);
            if (user == null) return;

            user.ResetToken = randomTokenString();
            user.ResetTokenExpires = DateTime.Now.AddHours(1);

            _context.Users.Update(user);
            _context.SaveChanges();

            sendPasswordResetEmail(user, origin);
        }

        public void ResetPassword(ResetPasswordRequestModel model)
        {
            var user = _context.Users.SingleOrDefault(x =>
                x.ResetToken == model.Token &&
                x.ResetTokenExpires > DateTime.Now);

            if (user == null)
                throw new AppException("Invalid token");

            //update passwrod
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(model.Password, out passwordHash, out passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.ResetToken = null;
            user.ResetTokenExpires = null;

            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public void Update(User userParams, string password = null)
        {
            var user = _context.Users.Find(userParams.UserId);

            if (user == null)
                throw new AppException("User not found");

            if (!string.IsNullOrWhiteSpace(userParams.Name))
                user.Name = userParams.Name;
            if (!string.IsNullOrWhiteSpace(userParams.Phone))
                user.Phone = userParams.Phone;
            if (!string.IsNullOrWhiteSpace(userParams.Role))
                user.Role = userParams.Role;

            //update password if provided
            if (!string.IsNullOrWhiteSpace(password))
            {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }
            user.Updated = DateTime.Now;

            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var user = _context.Users.Find(id);
            if(user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }

        //private helper method
        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null)
                throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }

        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computerHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for(int i = 0; i < computerHash.Length; i++)
                {
                    if (computerHash[i] != storedHash[i])
                        return false;
                }
            }
            return true;
        }

        private string randomTokenString()
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[40];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            // convert random bytes to hex string
            return BitConverter.ToString(randomBytes).Replace("-", "");
        }

        private void sendVerificationEmail(User user, string origin)
        {
            string message;
            if (!string.IsNullOrEmpty(origin))
            {
                var verifyUrl = $"{origin}/api/user/verify-email?token={user.VerificationToken}";
                message = $@"<p>Please click the below link to verify your email address:</p>
                             <p><a href=""{verifyUrl}"">{verifyUrl}</a></p>";
            }
            else
            {
                message = $@"<p>Please use the below token to verify your email address with the <code>/api/users/verify-email</code> api route:</p>
                             <p><code>{user.VerificationToken}</code></p>";
            }

            _emailService.Send(
                to: user.Email,
                subject: "To Do API - Verify Email",
                html: $@"<h4>Verify Email</h4>
                         <p>Thanks for registering!</p>
                         {message}"
            );
        }

        private void sendPasswordResetEmail(User user, string origin)
        {
            string message;
            if (!string.IsNullOrEmpty(origin))
            {
                var resetUrl = $"{origin}/api/users/reset-password?token={user.ResetToken}";
                message = $@"<p>Please click the below link to reset your password, the link will be valid for 1 day:</p>
                             <p><a href=""{resetUrl}"">{resetUrl}</a></p>";
            }
            else
            {
                message = $@"<p>Please use the below token to reset your password with the <code>/api/users/reset-password</code> api route:</p>
                             <p><code>{user.ResetToken}</code></p>";
            }

            _emailService.Send(
                to: user.Email,
                subject: "To Do API - Reset Password",
                html: $@"<h4>Reset Password Email</h4>
                         {message}"
            );
        }
    }
}
