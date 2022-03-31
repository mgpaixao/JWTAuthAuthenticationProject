using JWTAuthAuthentication3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.Repositories
{
    public interface IUserService: IDisposable
    {
        Task<IEnumerable<User>> GetUserWithRole();
        Task AddAsync(User entity);
        Task<User> GetById(int id);
        Task<User> EditName(User user, string name);
        Task DeleteUser(int id);

    }
}
