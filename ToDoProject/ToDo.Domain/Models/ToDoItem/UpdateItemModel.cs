using System;
using System.Collections.Generic;
using System.Text;

namespace ToDo.Domain.Models.ToDoItem
{
    public class UpdateItemModel
    {
        public string TaskName { get; set; }
        public string Content { get; set; }
        public DateTime DeadLine { get; set; }
        public bool IsComplete { get; set; }
    }
}
