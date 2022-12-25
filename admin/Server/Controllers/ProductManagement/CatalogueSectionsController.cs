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
    public class CatalogueSectionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CatalogueSectionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/CatalogueSections
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CatalogueSection>>> GetCatalogueSections([FromQuery] string companyId)
        {
            return await _context.CatalogueSection.Include(c => c.Catalogue).Where(x => x.Catalogue.CompanyId == companyId).ToListAsync();
        }

        // GET: api/CatalogueSections/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CatalogueSection>> GetCatalogueSection(string id)
        {
            var catalogueSection = await _context.CatalogueSection.FindAsync(id);

            if (catalogueSection == null)
            {
                return NotFound();
            }

            return catalogueSection;
        }

        // PUT: api/CatalogueSections/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCatalogueSection(string id, CatalogueSection catalogueSection)
        {
            if (id != catalogueSection.Id)
            {
                return BadRequest();
            }

            _context.Entry(catalogueSection).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CatalogueSectionExists(id))
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

        // POST: api/CatalogueSections
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CatalogueSection>> PostCatalogueSection(CatalogueSection catalogueSection)
        {

            if(CatalogueSectionNameExists(catalogueSection.Name, catalogueSection.CatalogueId))
            {
                catalogueSection = _context.CatalogueSection.FirstOrDefault(x => x.Name == catalogueSection.Name && x.CatalogueId == catalogueSection.CatalogueId);
                await PutCatalogueSection(catalogueSection.Id, catalogueSection);
                
            }
            else
            {
                _context.CatalogueSection.Add(catalogueSection);
                await _context.SaveChangesAsync();
            }
            

            return CreatedAtAction("GetCatalogueSection", new { id = catalogueSection.Id }, catalogueSection);
        }

        // DELETE: api/CatalogueSections/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCatalogueSection(string id)
        {
            var catalogueSection = await _context.CatalogueSection.FindAsync(id);
            if (catalogueSection == null)
            {
                return NotFound();
            }

            _context.CatalogueSection.Remove(catalogueSection);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CatalogueSectionExists(string id)
        {
            return _context.CatalogueSection.Any(e => e.Id == id);
        }

        private bool CatalogueSectionNameExists(string name, string catalogueId)
        {
            return _context.CatalogueSection.Any(e => e.CatalogueId == catalogueId && e.Name == name);
        }


    }
}
