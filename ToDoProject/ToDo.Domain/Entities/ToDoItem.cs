using System;
using System.Collections.Generic;
using System.Text;

namespace ToDo.Domain.Entities
{
    public class ToDoItem
    {
        public int ToDoItemId { get; set; }
        public string TaskName { get; set; }
        public string Content { get; set; }
        public DateTime DeadLine { get; set; }
        public bool IsComplete { get; set; }
        //public int UserId { get; set; }
        public User User { get; set; }
    }
}
