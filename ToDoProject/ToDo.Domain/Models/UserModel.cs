using System;
using System.Collections.Generic;
using System.Text;

namespace ToDo.Domain.Models
{
    public class UserModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string AvatarPath { get; set; }
        public string Role { get; set; }
        public bool IsVerified { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
    }
}
