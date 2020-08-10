using System;
using System.Collections.Generic;
using System.Text;
using ToDo.Domain.Entities;

namespace ToDo.Domain.Interfaces
{
    public interface IToDoItemRepository : IRepository<ToDoItem>
    {
        void Complete(int id);
        IEnumerable<ToDoItem> GetAllItemOfUser(int userId);
        int GetUserId(int id);
    }
}
