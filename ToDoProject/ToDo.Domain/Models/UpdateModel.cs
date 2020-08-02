using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ToDo.Domain.Models
{
    public class UpdateModel
    {
        [MinLength(6)]
        public string Password { get; set; }
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
    }
}
