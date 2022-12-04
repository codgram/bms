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
using Application.Model;
using Application.Model.Enum;
using System.Text;

namespace Application.Server.Controllers.ProductManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Item
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Item>>> GetItems([FromQuery] string companyId)
        {
            return await _context.Item.Include(i => i.Subgroup).Where(i => i.CompanyId == companyId).ToListAsync();
        }

        // GET: api/Item/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Item>> GetItem(string id)
        {
            var item = await _context.Item.FindAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            return item;
        }

        // PUT: api/Item/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutItem(string id, Item item)
        {
            if (id != item.Id)
            {
                return BadRequest();
            }

            _context.Entry(item).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ItemExists(id))
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

        // POST: api/Item
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Item>> PostItem(Item item)
        {
            _context.Item.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetItem", new { id = item.Id }, item);
        }

        // DELETE: api/Item/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(string id)
        {
            var item = await _context.Item.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            _context.Item.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("export/csv")]
        public async Task<FileContentResult> ExportCustomWorkLines([FromQuery] string companyId, [FromQuery] string? searchString)
        {
            var items = await _context.Item.Include(i => i.Subgroup).Include(i => i.Vendor)
                                        .Where(i => i.CompanyId == companyId)
                                        .Where(i => !String.IsNullOrEmpty(searchString) ? i.Code.Contains(searchString) || i.Brand.Contains(searchString) || i.Description.Contains(searchString) || i.Size.Contains(searchString) : true)
                                        .OrderBy(i => i.Code)
                                        .ToListAsync();

            var csv = new StringBuilder();
            csv.AppendLine("Code,Name,Brand,Description,Size,Subgroup,Vendor,Unit,Price,Stock");

            foreach (var item in items)
            {
                csv.AppendLine($"{item.Code},{item.Name},{item.Brand},{item.Description},{item.Size},{item.Subgroup.Name},{item.Vendor.Name}");
            }

            return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", "items.csv");
        }

        private bool ItemExists(string id)
        {
            return _context.Item.Any(e => e.Id == id);
        }
    }
}
