using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ToDo.Domain.Models
{
    public class VerifyEmailRequestModel
    {
        [Required]
        public string Token { get; set; }
    }
}
