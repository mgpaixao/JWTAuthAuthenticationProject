using JWTAuthAuthentication.Models;
using JWTAuthAuthentication.Services;
using SecureIdentity.Password;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace JWTAuthAuthentication.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        
        [Authorize(Roles = "admin")]
        [HttpPatch("v1/changeUserRole")]
        public async Task<IActionResult> ChangeRole(int UserId,
                                                    int NewRoleId,
                                                    [FromServices] DataContext context)
        {
            var user = await context
                .Users
                .FirstOrDefaultAsync(x => x.Id == UserId);

            var role = await context
                .Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == NewRoleId);

            if (user == null || role == null)
                return StatusCode(401, "Invalid Id");

            user.Roles = role;

            await context.SaveChangesAsync();
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("v1/register/")]
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

        [AllowAnonymous]
        [HttpPost("v1/accounts/login")]
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
    }
}
