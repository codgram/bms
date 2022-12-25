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
using Application.Model.Enum;

namespace Application.Server.Controllers.ProductManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogueItemsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CatalogueItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/CatalogueItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CatalogueItem>>> GetCatalogueItems([FromQuery] string companyId)
        {
            return await _context.CatalogueItem.Include(c => c.CatalogueSection).ThenInclude(c => c.Catalogue)
                                                .Include(c => c.Item).ThenInclude(c => c.SalesPrices.Where(s => s.Status == LineStatus.Active))
                                                .Where(x => x.CatalogueSection.Catalogue.CompanyId == companyId)
                                                .ToListAsync();
        }

        // GET: api/CatalogueItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CatalogueItem>> GetCatalogueItem(string id)
        {
            var catalogueItem = await _context.CatalogueItem.FindAsync(id);

            if (catalogueItem == null)
            {
                return NotFound();
            }

            return catalogueItem;
        }

        // PUT: api/CatalogueItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCatalogueItem(string id, CatalogueItem catalogueItem)
        {
            if (id != catalogueItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(catalogueItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CatalogueItemExists(id))
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

        // POST: api/CatalogueItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CatalogueItem>> PostCatalogueItem(CatalogueItem catalogueItem)
        {
            if (CatalogueItemCodeExists(catalogueItem.ItemId, catalogueItem.CatalogueSectionId))
            {
                catalogueItem = await _context.CatalogueItem.FirstOrDefaultAsync(x => x.ItemId == catalogueItem.ItemId && x.CatalogueSectionId == catalogueItem.CatalogueSectionId);
                await PutCatalogueItem(catalogueItem.Id, catalogueItem);
            }
            else
            {
                _context.CatalogueItem.Add(catalogueItem);
                await _context.SaveChangesAsync();
            }
            

            return CreatedAtAction("GetCatalogueItem", new { id = catalogueItem.Id }, catalogueItem);
        }

        // DELETE: api/CatalogueItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCatalogueItem(string id)
        {
            var catalogueItem = await _context.CatalogueItem.FindAsync(id);
            if (catalogueItem == null)
            {
                return NotFound();
            }

            _context.CatalogueItem.Remove(catalogueItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CatalogueItemExists(string id)
        {
            return _context.CatalogueItem.Any(e => e.Id == id);
        }
        private bool CatalogueItemCodeExists(string code, string CatalogueSectionId)
        {
            return _context.CatalogueItem.Any(e => e.ItemId == code && e.CatalogueSectionId == CatalogueSectionId);
        }
    }
}
