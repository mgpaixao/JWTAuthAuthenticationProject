using Domain.Interfaces.Repositories;
using JWTAuthAuthentication3;
using JWTAuthAuthentication3.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        private readonly DbSet<User> DbSet;
        public UserRepository(DataContext context) : base(context)
        {
            DbSet = context.Set<User>();
        }
        public async Task<IEnumerable<User>> GetUserWithRole()
        {
            return await DbSet
                .Include(x => x.Roles)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
