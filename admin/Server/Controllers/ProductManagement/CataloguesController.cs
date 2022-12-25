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
    public class CataloguesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CataloguesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Catalogues
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Catalogue>>> GetCatalogues([FromQuery] string companyId)
        {
            return await _context.Catalogue.Where(x => x.CompanyId == companyId).ToListAsync();
        }

        // GET: api/Catalogues/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Catalogue>> GetCatalogue(string id)
        {
            var catalogue = await _context.Catalogue.FindAsync(id);

            if (catalogue == null)
            {
                return NotFound();
            }

            return catalogue;
        }

        // PUT: api/Catalogues/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCatalogue(string id, Catalogue catalogue)
        {
            if (id != catalogue.Id)
            {
                return BadRequest();
            }

            _context.Entry(catalogue).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CatalogueExists(id))
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

        // POST: api/Catalogues
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Catalogue>> PostCatalogue(Catalogue catalogue)
        {
            if(CatalogueCodeExists(catalogue.Code, catalogue.CompanyId))
            {
                catalogue = await _context.Catalogue.FirstOrDefaultAsync(x => x.Code == catalogue.Code && x.CompanyId == catalogue.CompanyId);
                await PutCatalogue(catalogue.Id, catalogue);
            }
            else
            {
                _context.Catalogue.Add(catalogue);
                await _context.SaveChangesAsync();

                
            }

            return CreatedAtAction("GetCatalogue", new { id = catalogue.Id }, catalogue);


            
        }

        // DELETE: api/Catalogues/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCatalogue(string id)
        {
            var catalogue = await _context.Catalogue.FindAsync(id);
            if (catalogue == null)
            {
                return NotFound();
            }

            _context.Catalogue.Remove(catalogue);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CatalogueExists(string id)
        {
            return _context.Catalogue.Any(e => e.Id == id);
        }
        private bool CatalogueCodeExists(string code, string companyId)
        {
            return _context.Catalogue.Any(e => e.Code == code && e.CompanyId == companyId);
        }
    }
}
