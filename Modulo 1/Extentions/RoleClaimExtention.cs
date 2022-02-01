using JWTAuthAuthentication.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace JWTAuthAuthentication.Extentions
{
    public static class RoleClaimExtention
    {
        public static IEnumerable<Claim> GetClaims(this User user)
        {
            var result = new List<Claim>
            {
                new(ClaimTypes.Name, user.Name),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, user.Roles.Name)
            };
            //result.AddRange(
            //    user.Roles.Select(role => new Claim(ClaimTypes.Role, role.Name))
            //);
            return result;
        }
    }
}
