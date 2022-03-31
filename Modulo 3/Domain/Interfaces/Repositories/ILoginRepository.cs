using Domain.Repositories;
using JWTAuthAuthentication3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ILoginRepository : IBaseRepository<User>
    {
        Task<User?> GetUser(string email);
    }
}
