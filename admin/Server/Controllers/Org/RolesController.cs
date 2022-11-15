using System.Text;
using Application.Model.Org;
using Application.Server.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace Admin.Server.Controllers.Org;


// [Authorize]
[ApiController]
[Route("api/roles")]
public class RolesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public RolesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }


    // Get food pairing by Id
    [HttpGet]
    public async Task<ActionResult<IEnumerable<IdentityRole>>> GetRoleAsync()
    {
        return await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<IdentityRole>> GetRoleAsync(string id)
    {
        return await _roleManager.FindByIdAsync(id);
    }

    //Update user
    [HttpPut("{id}")]
    public async Task<IActionResult> PutUserAsync(string id, IdentityRole role)
    {
        var roleExists = await _roleManager.RoleExistsAsync(role.Name);
        if(!roleExists)
        {
            return NotFound();
        }

        await _roleManager.UpdateAsync(role);

        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult<IdentityRole>> PostRole(IdentityRole role)
    {
        var roleExists = await _roleManager.RoleExistsAsync(role.Name);
        if (roleExists)
        {
            return BadRequest();
        }

        var identityRole = new IdentityRole(role.Name);
        var result = await _roleManager.CreateAsync(identityRole);

        if (!result.Succeeded)
        {
            return BadRequest();
        }

        return role;
    }


    // DELETE: api/Entity/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRole(string id)
    {
        
        var role = await _roleManager.FindByIdAsync(id);
        if (role == null)
        {
            return NotFound();
        }

        var result = await _roleManager.DeleteAsync(role);

        if (!result.Succeeded)
        {
            return BadRequest();
        }

        return NoContent();
    }

    
}