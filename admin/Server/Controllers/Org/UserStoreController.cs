#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Application.Server.Data;
using Microsoft.AspNetCore.Identity;
using Application.Model.Org;

namespace Application.Server.Controllers.Org
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserStoreController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserStoreController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/UserStore
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserStore>>> GetUserStore()
        {
            var user = await _userManager.GetUserAsync(User);
            
            return await _context.UserStore.Include(u => u.Store).Where(u => u.ApplicationUserId == user.Id).OrderBy(u => u.Store.Code).ToListAsync();
        }

        // GET: api/UserStore
        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<UserStore>>> GetUserStoreByFilter([FromQuery] string userId)
        {
            
            return await _context.UserStore.Include(u => u.Store).Where(u => u.ApplicationUserId == userId).OrderBy(u => u.Store.Code).ToListAsync();
        }

        // GET: api/UserStore/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserStore>> GetUserStore(string id)
        {
            var userStore = await _context.UserStore.FindAsync(id);

            if (userStore == null)
            {
                return NotFound();
            }

            return userStore;
        }

        // PUT: api/UserStore/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserStore(string id, UserStore userStore)
        {
            if (id != userStore.Id)
            {
                return BadRequest();
            }

            _context.Entry(userStore).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserStoreExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/UserStore
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserStore>> PostUserStore(UserStore userStore)
        {
            _context.UserStore.Add(userStore);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserStore", new { id = userStore.Id }, userStore);
        }

        // DELETE: api/UserStore/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserStore(string id)
        {
            var userStore = await _context.UserStore.FindAsync(id);
            if (userStore == null)
            {
                return NotFound();
            }

            _context.UserStore.Remove(userStore);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserStoreExists(string id)
        {
            return _context.UserStore.Any(e => e.Id == id);
        }
    }
}
