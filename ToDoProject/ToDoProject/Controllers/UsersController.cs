using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ToDo.API.Helpers;
using ToDo.API.Models;
using ToDo.Domain.Entities;
using ToDo.Domain.Helpers;
using ToDo.Domain.Interfaces;
using ToDo.Domain.Models;

namespace ToDo.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUserRepository _userService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public UsersController(
            IUserRepository userService,
            IMapper mapper,
            IOptions<AppSettings> appSettings)
        {
            _userService = userService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }
        /// <summary>
        /// Login
        /// </summary>
        /// <remarks>
        /// 
        /// Sample request:
        /// 
        ///     POST /Users/authenticate
        ///     {
        ///         "UserName" : "user1",
        ///         "Password" "user123"
        ///     }
        /// </remarks>
        /// <param name="model"></param>
        /// <returns>User's info and token</returns>
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]AuthenticateModel model)
        {
            var user = _userService.Authenticate(model.UserName, model.Password);

            if(user == null)
                return BadRequest(new { message = "Username or password is incorrect" });
            if(!user.IsVerified)
                return BadRequest(new { message = "Email have not been verified" });

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserId.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                Phone = user.Phone,
                Role = user.Role,
                IsVerified = user.IsVerified,
                Created = user.Created,
                Update = user.Updated,
                Token = tokenString
            });
        }

        /// <summary>
        /// Register new user
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Notification string</returns>
        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody]RegisterModel model)
        {
            var user = _mapper.Map<User>(model);

            try
            {
                _userService.Create(user, model.Password, Request.Headers["origin"]);
                return Ok(new { message = "Registration successful, please check your email for verification instructions" });
            }
            catch(AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        /// <summary>
        /// verify email
        /// </summary>
        /// <param name="model"></param>
        /// <returns>notification string</returns>
        [AllowAnonymous]
        [HttpPost("verify-email")]
        public IActionResult VerifyEmail(VerifyEmailRequestModel model)
        {
            _userService.VerifyEmail(model.Token);
            return Ok(new { message = "Verification successful, you can now login" });
        }
        
        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword(ForgotPasswordRequestModel model)
        {
            _userService.ForgotPassword(model, Request.Headers["origin"]);
            return Ok(new { message = "Please check your email for password reset instructions" });
        }

        /// <summary>
        /// reset password when using forgot-password
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Notification string</returns>
        [AllowAnonymous]
        [HttpPost("reset-password")]
        public IActionResult ResetPassword(ResetPasswordRequestModel model)
        {
            _userService.ResetPassword(model);
            return Ok(new { message = "Password reset successful, you can now login" });

        }

        /// <summary>
        /// Get a users list
        /// </summary>
        /// <returns>users list</returns>
        [Authorize(Roles = Role.Admin)]
        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            var model = _mapper.Map<IList<UserModel>>(users);
            return Ok(model);
        }

        /// <summary>
        /// Get a user by userId
        /// </summary>
        /// <param name="id"></param>
        /// <returns>a user info</returns>
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var currentUserId = int.Parse(User.Identity.Name);
            if (id != currentUserId && !User.IsInRole(Role.Admin))
                return Forbid();

            var user = _userService.GetById(id);
            
            if (user == null)
                return NotFound();
            var model = _mapper.Map<UserModel>(user);

            return Ok(model);
        }

        /// <summary>
        /// update info of user
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody]UpdateModel model)
        {
            var user = _mapper.Map<User>(model);
            user.UserId = id;

            var currentUserId = int.Parse(User.Identity.Name);
            if (id != currentUserId && !User.IsInRole(Role.Admin))
                return Forbid();

            try
            {
                _userService.Update(user, model.Password);
                return Ok();
            }
            catch(AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Delete user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles =Role.Admin)]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _userService.Delete(id);
            return Ok();
        }
    }
}