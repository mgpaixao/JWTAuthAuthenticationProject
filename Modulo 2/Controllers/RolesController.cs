using JWTAuthAuthentication2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace JWTAuthAuthentication2.Controllers;

[Authorize(Roles = "admin")]
[ApiController]
[Route("api/Roles")]
public class RolesController : ControllerBase
{
    #region Post AddRole
    [HttpPost("AddRole")]
    public async Task<IActionResult> AddRole(RoleViewModel roleRequest,
                                             [FromServices] DataContext context)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (context.Roles.Any(x => x.Name.Equals(roleRequest.Name))) return BadRequest("Role duplicated");

        var roleModel = new Role
        {
            Name = roleRequest.Name,
        };
        
        await context.Roles.AddAsync(roleModel);
        await context.SaveChangesAsync();
        return Ok(roleModel);
    }
    #endregion

    #region Patch Change User Role
    [HttpPatch("ChangeUserRole")]
    public async Task<IActionResult> ChangeUserRole(int UserId,
                                                    int NewRoleId,
                                                    [FromServices] DataContext context)
    {
        if(!ModelState.IsValid) return BadRequest(ModelState);

        //Consultas
        var userModel = await context
            .Users
            .FirstOrDefaultAsync(x => x.Id == UserId);

        var roleModel = await context
            .Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == NewRoleId);

        //Validações
        if (userModel == null || roleModel == null)
            return StatusCode(401, "Invalid Id");

        //Verificando quantos usuarios admins temos no sistema
        var userList = await context.Users.ToListAsync();
        int adminUsers = 0;
        foreach (var item in userList)
        {
            if (item.RolesId == 2) adminUsers++;
        }

        //Verificando se sobrará pelo menos 1 admin no sistema
        if (userModel.RolesId == 2 && adminUsers <= 1) return StatusCode(400, "You must have at least 1 admin user in the database");

        userModel.Roles = roleModel;

        //Admin não pode mudar sua própria role
        if (User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value.Equals(userModel.Id.ToString())) return StatusCode(400, "You cannot change your own role");

        
        await context.SaveChangesAsync();
        return Ok(userModel.Name + " Role changed to " + roleModel.Name);
    }
    #endregion

    #region Get Roles
    [HttpGet("GetRoles")]
    public async Task<IEnumerable<Role>> GetRoles([FromServices] DataContext context)
    {
        return await context.Roles.ToListAsync();
    }
    #endregion

    #region Edit Roles
    [HttpPatch("EditRole")]
    public async Task<IActionResult> EditRole([FromServices] DataContext context,
                                                              Role roleRequest)
    {
        if(!ModelState.IsValid) return BadRequest(ModelState);

        var roleModel = await context.Roles.AsNoTracking().FirstOrDefaultAsync(x => x.Id == roleRequest.Id);

        //Validações
        if (roleModel == null) return NotFound("Role not found");
        if (context.Roles.Any(x => x.Name.Equals(roleRequest.Name))) return BadRequest("Role duplicated");


        context.Roles.Update(roleRequest);
        await context.SaveChangesAsync(); 
        return Ok(roleRequest);        
    }
    #endregion

    #region Delete Roles
    [HttpDelete("DeleteRoles")]
    public async Task<IActionResult> DeleteRoles(int roleId,
                                                [FromServices] DataContext context)
    {
        if(!ModelState.IsValid) return BadRequest(ModelState);

        var roleModel = await context.Roles.FirstOrDefaultAsync(x => x.Id == roleId);

        //Validações
        if (roleModel == null) return NotFound("Role not found");
        if (roleModel.Name.Contains("admin")) return BadRequest("Cannot delete admin role");

        context.Roles.Remove(roleModel);
        await context.SaveChangesAsync();
        return Ok("Role " + roleModel.Name + " Deleted");
    }
    #endregion

}

