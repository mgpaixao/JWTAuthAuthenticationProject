using System.Collections.Generic;

namespace JWTAuthAuthentication2.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Slug { get; set; }
        public int RolesId { get; set; }
        public Role Roles { get; set; }
    }
}
