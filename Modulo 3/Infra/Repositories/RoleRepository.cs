using Domain.Interfaces;
using JWTAuthAuthentication3;
using JWTAuthAuthentication3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Repositories
{
    public class RoleRepository : BaseRepository<Role>, IRoleRepository
    {
        public RoleRepository(DataContext context) : base(context)
        {

        }
    }
}
