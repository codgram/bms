#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Application.Model.ProductManagement;
using Application.Server.Data;

namespace Application.Server.Controllers.ProductManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubgroupsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SubgroupsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Subgroups
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Subgroup>>> GetSubgroups([FromQuery] string companyId)
        {
            return await _context.Subgroup.Include(s => s.Group).ThenInclude(g => g.Category).ThenInclude(c => c.Division).Where(x => x.Group.Category.Division.CompanyId == companyId).ToListAsync();
        }

        // GET: api/Subgroups/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Subgroup>> GetSubgroup(string id)
        {
            var subgroup = await _context.Subgroup.FindAsync(id);

            if (subgroup == null)
            {
                return NotFound();
            }

            return subgroup;
        }

        // PUT: api/Subgroups/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSubgroup(string id, Subgroup subgroup)
        {
            if (id != subgroup.Id)
            {
                return BadRequest();
            }

            _context.Entry(subgroup).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubgroupExists(id))
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

        // POST: api/Subgroups
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Subgroup>> PostSubgroup(Subgroup subgroup)
        {
            _context.Subgroup.Add(subgroup);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSubgroup", new { id = subgroup.Id }, subgroup);
        }

        // DELETE: api/Subgroups/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubgroup(string id)
        {
            var subgroup = await _context.Subgroup.FindAsync(id);
            if (subgroup == null)
            {
                return NotFound();
            }

            _context.Subgroup.Remove(subgroup);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SubgroupExists(string id)
        {
            return _context.Subgroup.Any(e => e.Id == id);
        }
    }
}
