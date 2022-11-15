using System.Text;
using Application.Server.Data;
using Application.Model.Org;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace Admin.Server.Controllers.WineApp;


// [Authorize]
[ApiController]
[Route("api/userroles")]
public class UserRolesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserRolesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }




    [HttpGet("{id}")]
    public async Task<List<IdentityUserRole<string>>> GetUserRoleAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        return await _context.UserRoles.Where(x => x.UserId == user.Id).OrderBy(u => u.UserId).ToListAsync();

    }

    //Update user
    [HttpPut("{id}")]
    public async Task<IActionResult> PutUserRoleAsync(string id, string[] roleNames)
    {
        //Change user role
        var user = await _userManager.FindByIdAsync(id);
        var currentRoles = await _userManager.GetRolesAsync(user);

        // remove current roles
        var result = await _userManager.RemoveFromRolesAsync(user, currentRoles);

        // add rolenames
        result = await _userManager.AddToRolesAsync(user, roleNames);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return Ok();
        
    }

    [HttpPost("{id}")]
    public async Task<ActionResult<IdentityResult>> PostUserRole(string id, string[] roleNames)
    {
        // add role to user
        var user = await _userManager.FindByIdAsync(id);

        if(user == null)
        {
            return NotFound();
        }

        return await _userManager.AddToRolesAsync(user, roleNames);; 
        
    }


    // DELETE: api/Entity/5
    [HttpDelete("{id}/{name}")]
    public async Task<IActionResult> DeleteUserRole(string id, string name)
    {
        
        var user = await _userManager.FindByIdAsync(id);

        if(user == null)
        {
            return NotFound();
        }

        var result = await _userManager.RemoveFromRoleAsync(user, name);

        if(!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return Ok();
    }

    
}