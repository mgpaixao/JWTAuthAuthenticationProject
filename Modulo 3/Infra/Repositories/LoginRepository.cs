using Domain.Interfaces;
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
    public class LoginRepository : BaseRepository<User>, ILoginRepository
    {
        private readonly DbSet<User> DbSet;
        public LoginRepository(DataContext context) : base(context)
        {
            DbSet = context.Set<User>();
        }

        public async Task<User?> GetUser(string email)
        {
            return await DbSet
                .Include(x => x.Roles)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Email == email);
        }

    }
}
