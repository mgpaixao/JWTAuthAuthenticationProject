using JWTAuthAuthentication3.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.Services
{
    public interface IRoleService : IDisposable
    {
        Task<Role> GetById(Guid id);
        Task<IEnumerable<Role>> GetAllAsync();
        Task<bool> Exists(Expression<Func<Role, bool>> expression);
        Task AddAsync(Role entity);
        Task<string> ChangeUserRole(int userId, int newRoleId, string? userClaim);
        Task<string> EditRole(Role roleRequest);
        Task<string> DeleteRole(int roleId);
        void Update(Role entity);
        void Remove(Role entity);
    }
}
