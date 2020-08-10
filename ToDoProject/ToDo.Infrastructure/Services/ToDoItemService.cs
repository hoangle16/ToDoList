using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToDo.Domain.Entities;
using ToDo.Domain.Helpers;
using ToDo.Domain.Interfaces;
using ToDo.Infrastructure.Data;

namespace ToDo.Infrastructure.Services
{
    public class ToDoItemService : IToDoItemRepository
    {
        private ToDoContext _context;
        public ToDoItemService(ToDoContext context)
        {
            _context = context;
        }
        public ToDoItem Add(ToDoItem entity)
        {
            if (string.IsNullOrWhiteSpace(entity.TaskName))
                throw new AppException("Task Name is required");
            entity.Created = DateTime.Now;
            entity.IsComplete = false;

            _context.ToDoItems.Add(entity);
            _context.SaveChanges();

            return entity;
        }

        public void Complete(int id)
        {
            var item = _context.ToDoItems.Find(id);
            item.IsComplete = true;
            item.Updated = DateTime.Now;
            _context.ToDoItems.Update(item);
            _context.SaveChanges();
        }

        public IEnumerable<ToDoItem> GetAll()
        {
            return _context.ToDoItems;
        }

        public ToDoItem GetById(int id)
        {
            return _context.ToDoItems.Find(id);
        }

        public IEnumerable<ToDoItem> GetAllItemOfUser(int id)
        {
            var items = _context.ToDoItems.Where(e => e.UserId == id);

            return items;
        }

        public void Remove(int id)
        {
            var item = _context.ToDoItems.Find(id);
            if(item != null)
            {
                _context.ToDoItems.Remove(item);
                _context.SaveChanges();
            }
        }

        public void Update(ToDoItem entity)
        {
            var item = _context.ToDoItems.Find(entity.ToDoItemId);
            if (!string.IsNullOrWhiteSpace(entity.TaskName))
                item.TaskName = entity.TaskName;
            if (!string.IsNullOrWhiteSpace(entity.Content))
                item.Content = entity.Content;
            if (entity.DeadLine != null)
                item.DeadLine = entity.DeadLine;
            item.Updated = DateTime.Now;
            item.IsComplete = entity.IsComplete;

            _context.Update(item);
            _context.SaveChanges();
        }
        
        //helper method
        //get UserId by ItemId
        public int GetUserId(int id)
        {
            return _context.ToDoItems.Find(id).UserId;
        }
    }
}
