using System;
using System.Collections.Generic;
using System.Text;

namespace ToDo.Domain.Models.ToDoItem
{
    public class ItemModel
    {
        public int ToDoItemId { get; set; }
        public string TaskName { get; set; }
        public string Content { get; set; }
        public DateTime DeadLine { get; set; }
        public bool IsComplete { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public int UserId { get; set; }
    }
}
