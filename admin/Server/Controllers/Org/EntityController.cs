#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Application.Server.Data;
using Application.Model.Org;

namespace Application.Server.Controllers.Org
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntityController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EntityController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Entity
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Entity>>> GetEntities([FromQuery] string companyId)
        {
            return await _context.Entity.Where(e => e.CompanyId == companyId).OrderBy(e => e.Code).ToListAsync();
        }

        // GET: api/Entity/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Entity>> GetEntity(string id)
        {
            var entity = await _context.Entity.FindAsync(id);

            if (entity == null)
            {
                return NotFound();
            }

            return entity;
        }

        // PUT: api/Entity/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEntity(string id, Entity entity)
        {
            if (id != entity.Id)
            {
                return BadRequest();
            }

            _context.Entry(entity).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EntityExists(id))
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

        // POST: api/Entity
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Entity>> PostEntity(Entity entity)
        {
            _context.Entity.Add(entity);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEntity", new { id = entity.Id }, entity);
        }

        // DELETE: api/Entity/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEntity(string id)
        {
            var entity = await _context.Entity.FindAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            _context.Entity.Remove(entity);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EntityExists(string id)
        {
            return _context.Entity.Any(e => e.Id == id);
        }
    }
}
