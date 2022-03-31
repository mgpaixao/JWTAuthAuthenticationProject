using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services.Interfaces
{
    public interface ILoginService : IDisposable
    {
        Task<string> LoginAsync(string email, string password);
    }
}
