using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ToDo.Domain.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
        public string VerificationToken { get; set; }
        public bool IsVerified { get; set; }
        public string ResetToken { get; set; }
        public DateTime? ResetTokenExpires { get; set; }
        public string AvatarPath { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public virtual ICollection<ToDoItem> TodoItems { get; set; }
    }
}
