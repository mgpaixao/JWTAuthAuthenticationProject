using JWTAuthAuthentication3.Models;
using JWTAuthAuthentication3.Services;
using SecureIdentity.Password;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Domain.Services;
using Domain.Services.Interfaces;

namespace JWTAuthAuthentication3.Controllers
{
    [Authorize]
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILoginService _loginService;
        public AuthController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        #region Login
        [AllowAnonymous]
        [HttpPost("v1/login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto login)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values);

            return Ok(await _loginService.LoginAsync(login.Email, login.Password));
        }
        #endregion
    }
}
