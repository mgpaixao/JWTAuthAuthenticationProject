using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IBaseRepository<T> : IDisposable where T : Entity
    {
        Task<T> GetById(Guid id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetOneBy(Expression<Func<T, bool>> expression);
        Task<IEnumerable<T>> GetListBy(Expression<Func<T, bool>> expression);
        Task<bool> Exists(Expression<Func<T, bool>> expression);
        Task<int> SaveChanges();
        Task AddAsync(T entity);
        public void Update(T entity);
        public void Remove(T entity);
    }
}
