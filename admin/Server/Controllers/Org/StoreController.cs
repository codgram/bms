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
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Application.Model.Org;

namespace Application.Server.Controllers.Org
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StoreController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Store
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Store>>> GetStores([FromQuery] string companyId)
        {
            return await _context.Store.Include(s => s.Entity).Where(s => s.Entity.CompanyId == companyId).OrderBy(s => s.Entity.Code).ThenBy(s => s.Code).ToListAsync();
        }

        // GET: api/Store/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Store>> GetStore(string id)
        {
            var store = await _context.Store.FindAsync(id);

            if (store == null)
            {
                return NotFound();
            }

            return store;
        }

        // PUT: api/Store/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStore(string id, Store store)
        {
            if (id != store.Id)
            {
                return BadRequest();
            }

            _context.Entry(store).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StoreExists(id))
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

        // POST: api/Store
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Store>> PostStore(Store store)
        {
            _context.Store.Add(store);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStore", new { id = store.Id }, store);
        }

        // DELETE: api/Store/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStore(string id)
        {
            var store = await _context.Store.FindAsync(id);
            if (store == null)
            {
                return NotFound();
            }

            _context.Store.Remove(store);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StoreExists(string id)
        {
            return _context.Store.Any(e => e.Id == id);
        }
    }
}
