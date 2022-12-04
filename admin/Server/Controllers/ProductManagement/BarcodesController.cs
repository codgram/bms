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
    public class BarcodesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BarcodesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Barcode
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Barcode>>> GetBarcodes([FromQuery] string companyId)
        {
            return await _context.Barcode.Include(i => i.Item).Where(i => i.CompanyId == companyId).ToListAsync();
        }

        // GET: api/Barcode/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Barcode>> GetBarcode(string id)
        {
            var barcode = await _context.Barcode.Include(i => i.Item).FirstOrDefaultAsync(i => i.Id == id);

            if (barcode == null)
            {
                return NotFound();
            }

            return barcode;
        }

        // PUT: api/Barcode/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBarcode(string id, Barcode barcode)
        {
            if (id != barcode.Id)
            {
                return BadRequest();
            }

            _context.Entry(barcode).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BarcodeExists(id))
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

        // POST: api/Barcode
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Barcode>> PostBarcode(Barcode barcode)
        {
            _context.Barcode.Add(barcode);
            await _context.SaveChangesAsync();

            // subgroup of the barcode
            var item = await _context.Subgroup.FindAsync(barcode.ItemId);

            return CreatedAtAction("GetBarcode", new { id = barcode.Id, item = barcode.Item }, barcode);
        }

        // DELETE: api/Barcode/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBarcode(string id)
        {
            var barcode = await _context.Barcode.FindAsync(id);
            if (barcode == null)
            {
                return NotFound();
            }

            _context.Barcode.Remove(barcode);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BarcodeExists(string id)
        {
            return _context.Barcode.Any(e => e.Id == id);
        }

        private bool BarcodeCodeExists(string companyId, string barcodeNo)
        {
            return _context.Barcode.Any(e => e.CompanyId == companyId && e.BarcodeNo == barcodeNo);
        }
    }
}
