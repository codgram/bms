using System.Security.Claims;
using System.Text;
using Application.Server.Data;
using Application.Model.Org;
using Application.Model.Org;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace Admin.Server.Controllers.Org;


// [Authorize]
[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserStore<ApplicationUser> _userStore;
    private readonly IUserEmailStore<ApplicationUser> _emailStore;
    private readonly IConfiguration _configuration;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UsersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IUserStore<ApplicationUser> userStore, IConfiguration configuration, RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _userStore = userStore;
        _configuration = configuration;
        _roleManager = roleManager;
        _emailStore = GetEmailStore();
    }


    // Get food pairing by Id
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ApplicationUser>>> GetUsersAsync()
    {
        return await _context.ApplicationUser.OrderBy(u => u.UserName).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApplicationUser>> GetUserByIdAsync(string id)
    {
        return await _userManager.FindByIdAsync(id);
    }



    // Add new user
    [HttpPost]
    public async Task<ActionResult<ApplicationUser>> AddUserAsync(UserInputModel userInputModel)
    {
        var user = new ApplicationUser() {
            UserName = userInputModel.UserName,
            Email = userInputModel.Email,
            FirstName = userInputModel.FirstName,
            LastName = userInputModel.LastName,
            EmailConfirmed = true,
            Password = userInputModel.Password
        };

        // user.EmailConfirmed = true;
        if(!UsernameExists(user.UserName) && !EmailExists(user.Email)) {
            await _userStore.SetUserNameAsync(user, user.UserName, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, user.Email, CancellationToken.None);

            await _userManager.CreateAsync(user, user.Password);
            var userId = await _userManager.GetUserIdAsync(user);

            return user;
        }
        else {
            return StatusCode(406, $"The username or email already exists.");  
        }

    }


    [HttpPut("{id}")]
    public async Task<ActionResult<ApplicationUser>> UpdateUserAsync(string id, UserInputModel userInputModel)
    {

        var user = await _userManager.FindByNameAsync(userInputModel.UserName);

        if (id != user.Id)
        {
            return BadRequest();
        }

        user.UserName = userInputModel.UserName;
        user.Email = userInputModel.Email;
        user.FirstName = userInputModel.FirstName;
        user.LastName = userInputModel.LastName;

        
        try
        {
            await _userManager.UpdateAsync(user);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!UserExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return user;

    }



    [HttpPut("password/reset/{id}")]
    public async Task<ActionResult<ApplicationUser>> ResetPasswordAsync(string id, UserInputModel userInput)
    {
        var user = await _userManager.FindByNameAsync(userInput.UserName);

        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }
        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        var newCode = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

        var addPasswordResult = await _userManager.ResetPasswordAsync(user, newCode ,userInput.Password);


        if(!addPasswordResult.Succeeded) {
            foreach (var error in addPasswordResult.Errors)
            {
                Console.WriteLine($"Error: {error.Description}");
            }
        }
        return user;

    }


    // delete
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApplicationUser>> DeleteUserAsync(string id)
    {
        // get user
        var user = await _userManager.FindByIdAsync(id);

        // get user roles
        var userRoles = await _userManager.GetRolesAsync(user);

        // delete all user roles
        foreach (var userRole in userRoles)
        {
            await _userManager.RemoveFromRoleAsync(user, userRole);
        }

        if (user == null)
        {
            return NotFound();
        }

        await _userManager.DeleteAsync(user);

        await _context.SaveChangesAsync();
        return user;
    }



    private bool UserExists(string id)
    {
        return _context.ApplicationUser.Any(e => e.Id == id);
    }

    private bool UsernameExists(string username)
    {
        return _context.ApplicationUser.Any(e => e.UserName == username);
    }

    private bool EmailExists(string email)
    {
        return _context.ApplicationUser.Any(e => e.Email == email);
    }

    private IUserEmailStore<ApplicationUser> GetEmailStore()
    {
        if (!_userManager.SupportsUserEmail)
        {
            throw new NotSupportedException("The default UI requires a user store with email support.");
        }
        return (IUserEmailStore<ApplicationUser>)_userStore;
    }
    
}