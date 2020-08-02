using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ToDo.Domain.Models
{
    public class ForgotPasswordRequestModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
