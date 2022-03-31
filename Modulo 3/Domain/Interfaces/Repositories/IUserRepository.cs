using Domain.Repositories;
using JWTAuthAuthentication3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.Repositories
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<IEnumerable<User>> GetUserWithRole();
        Task<int> SaveChanges();
        void Update(User entity);
        void Remove(User entity);
    }
}
