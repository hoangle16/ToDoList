using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ToDo.Domain.Interfaces
{
    public interface IRepository<T>
    {
        Task<T> GetById(int id);
        Task<IEnumerable<T>> GetAll();

        Task<T> Add(T entity);
        void Update(T entity);
        void Remove(T entity);
        
    }
}
