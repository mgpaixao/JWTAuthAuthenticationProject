using Domain.Models;
using Domain.Repositories;
using JWTAuthAuthentication3;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : Entity
    {
        private readonly DbSet<T> DbSet;
        private readonly DataContext _context;

        protected BaseRepository(DataContext context)
        {
            _context = context;
            DbSet = _context.Set<T>();
        }

        public async Task<T?> GetById(Guid id) =>
        await DbSet.FindAsync(id);

        public async Task<IEnumerable<T>> GetAllAsync() =>
            await DbSet.ToListAsync();

        public async Task<T?> GetOneBy(Expression<Func<T, bool>> expression) =>
            await DbSet.AsNoTracking().FirstOrDefaultAsync(expression);

        public async Task<IEnumerable<T>> GetListBy(Expression<Func<T, bool>> expression) =>
            await DbSet.Where(expression).ToListAsync();

        public async Task<bool> Exists(Expression<Func<T, bool>> expression) =>
            await DbSet.AnyAsync(expression);

        public async Task AddAsync(T entity)
        {
            await DbSet.AddAsync(entity);
            await SaveChanges();
        }

        public void Update(T entity) =>
            DbSet.Update(entity);

        public void Remove(T entity) =>
            DbSet.Remove(entity);

        public async Task<int> SaveChanges()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
