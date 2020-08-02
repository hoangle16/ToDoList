using System;
using System.Collections.Generic;
using System.Text;
using ToDo.Domain.Entities;
using ToDo.Domain.Models;

namespace ToDo.Domain.Interfaces
{
    public interface IUserRepository
    {
        User Authenticate(string username, string password);
        IEnumerable<User> GetAll();
        User GetById(int id);
        User Create(User user, string password, string origin);
        void Update(User user, string password = null);
        void Delete(int id);

        void VerifyEmail(string token);
        void ForgotPassword(ForgotPasswordRequestModel model, string origin);
        void ResetPassword(ResetPasswordRequestModel model);
    }
}
