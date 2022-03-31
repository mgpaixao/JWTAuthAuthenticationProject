using Domain.Interfaces.Services;
using JWTAuthAuthentication3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace JWTAuthAuthentication3.Controllers;

[Authorize(Roles = "admin")]
[ApiController]
[Route("api/Roles")]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;
    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }
    #region Post AddRole
    [HttpPost("AddRole")]
    public async Task<IActionResult> AddRole(RoleRequestDto roleRequest)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (await _roleService.Exists(x => x.Name.Equals(roleRequest.Name))) return BadRequest("Role duplicated");

        var roleModel = new Role
        {
            Name = roleRequest.Name,
        };
        
        await _roleService.AddAsync(roleModel);
        return Ok(roleModel);
    }
    #endregion

    #region Patch Change User Role
    [HttpPatch("ChangeUserRole")]
    public async Task<IActionResult> ChangeUserRole(int userId,
                                                    int newRoleId                                       )
    {
        if(!ModelState.IsValid) return BadRequest(ModelState);

        var userClaim = User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;

        return Ok(await _roleService.ChangeUserRole(userId, newRoleId, userClaim));
    }
    #endregion

    #region Get Roles
    [HttpGet("GetRoles")]
    public async Task<IEnumerable<Role>> GetRoles([FromServices] DataContext context)
    {
        return await _roleService.GetAllAsync();
    }
    #endregion

    #region Edit Roles
    [HttpPatch("EditRoleName")]
    public async Task<IActionResult> EditRole(Role roleRequest)
    {
        if(!ModelState.IsValid) return BadRequest(ModelState);
                
        return Ok(await _roleService.EditRole(roleRequest));        
    }
    #endregion

    #region Delete Roles
    [HttpDelete("DeleteRoles")]
    public async Task<IActionResult> DeleteRoles(int roleId)
    {
        if(!ModelState.IsValid) return BadRequest(ModelState);

        return Ok(await _roleService.DeleteRole(roleId));
    }
    #endregion

}

