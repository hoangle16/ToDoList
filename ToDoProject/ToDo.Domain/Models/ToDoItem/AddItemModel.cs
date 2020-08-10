using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ToDo.Domain.Models.ToDoItem
{
    public class AddItemModel
    {
        [Required]
        public string TaskName { get; set; }
        public string Content { get; set; }
        public DateTime DeadLine { get; set; }
    }
}
