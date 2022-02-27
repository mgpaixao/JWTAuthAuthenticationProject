using JWTAuthAuthentication2.Models;
using JWTAuthAuthentication2.Services;
using SecureIdentity.Password;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace JWTAuthAuthentication2.Controllers
{
    [Authorize]
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        #region Register
        [AllowAnonymous]
        [HttpPost("v1/register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model,
                                              [FromServices] DataContext context)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = new User
            {
                Name = model.Name,
                Email = model.Email,
                Slug = model.Email.Replace("@", "-").Replace(".", "-"),
                RolesId = 1,
                PasswordHash = PasswordHasher.Hash(model.Password),
            };

            try
            {
                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();

                return Ok($"{user.Email} {user.PasswordHash}");
            }
            catch (DbUpdateException)
            {
                return StatusCode(400, "Duplicate Email");
            }
            catch
            {
                return StatusCode(500, "Internal Error");
            }
        }
        #endregion

        #region Login
        [AllowAnonymous]
        [HttpPost("v1/login")]
        public async Task<IActionResult> Login(
       [FromBody] LoginViewModel model,
       [FromServices] DataContext context,
       [FromServices] TokenService tokenService)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values);

            var user = await context
                .Users
                .Include(x => x.Roles)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Email == model.Email);

            if (user == null)
                return StatusCode(401, "User or password invalid");

            if (!PasswordHasher.Verify(user.PasswordHash, model.Password))
                return StatusCode(401, "User or password invalid");

            try
            {
                var token = tokenService.GenerateToken(user);
                return Ok(token);
            }
            catch
            {
                return StatusCode(500, "Internal Error");
            }
        }
        #endregion
    }
}
