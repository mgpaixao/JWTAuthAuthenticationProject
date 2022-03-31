using Domain.Models;
using System.Collections.Generic;

namespace JWTAuthAuthentication3.Models
{
    public class Role : Entity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}