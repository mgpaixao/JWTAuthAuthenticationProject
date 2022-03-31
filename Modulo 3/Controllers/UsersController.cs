using Domain.Interfaces.Repositories;
using JWTAuthAuthentication3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace JWTAuthAuthentication3.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "admin")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userservice;
        public UsersController(IUserService userservice)
        {
            _userservice = userservice;
        }

        #region Post User Register
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto model)
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
                await _userservice.AddAsync(user);
           

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

        #region Get All Users
        [HttpGet]
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _userservice.GetUserWithRole();
        }
        #endregion

        [AllowAnonymous]
        #region Patch Edit User Name
        [HttpPatch("EditUserName")]
        public async Task<IActionResult> EditUserName(int id, string newName)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userservice.GetById(id);
            await _userservice.EditName(user, newName);

            return Ok(user);
        }
        #endregion

        #region Delete User
        [HttpDelete("DeleteUser")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);            

            await _userservice.DeleteUser(id);
            return Ok("Deleted");
        }
        #endregion
    }
}
