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
using Application.Model.Procurement;
using Application.Model.Enum;

namespace Application.Server.Controllers.ProductManagement
{
    [Route("api/purchase/headers")]
    [ApiController]
    public class PurchaseHeadersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PurchaseHeadersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/PurchaseHeaders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PurchaseHeader>>> GetPurchaseHeaders([FromQuery] string companyId)
        {
            return await _context.PurchaseHeader.Include(p => p.Vendor).Where(x => x.CompanyId == companyId).ToListAsync();
        }

        // GET: api/PurchaseHeaders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PurchaseHeader>> GetPurchaseHeader(string id, string? vendor)
        {
            var purchaseHeader = await _context.PurchaseHeader.Include(p => p.Vendor).FirstOrDefaultAsync(p => p.Id == id);

            if (purchaseHeader == null)
            {
                return NotFound();
            }

            return purchaseHeader;
        }

        // PUT: api/PurchaseHeaders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPurchaseHeader(string id, PurchaseHeader purchaseHeader)
        {

            if(purchaseHeader.Status != DocumentStatus.Open)
            {
                return BadRequest("Cannot edit a document with status other than Open");
            }

            if (id != purchaseHeader.Id)
            {
                return BadRequest();
            }

            _context.Entry(purchaseHeader).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PurchaseHeaderExists(id))
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

        // POST: api/PurchaseHeaders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PurchaseHeader>> PostPurchaseHeader(PurchaseHeader purchaseHeader)
        {
            
            // check if document no exists
            var documentNo = await _context.PurchaseHeader.Where(x => x.CompanyId == purchaseHeader.CompanyId).OrderByDescending(x => x.DocumentNo).FirstOrDefaultAsync();

            // if exists return error
            if (documentNo != null)
            {
                return BadRequest("Document No already exists");
            }

            _context.PurchaseHeader.Add(purchaseHeader);
            await _context.SaveChangesAsync();

            var vendor = await _context.Vendor.FindAsync(purchaseHeader.VendorId);

            return CreatedAtAction("GetPurchaseHeader", new { id = purchaseHeader.Id, vendor = vendor }, purchaseHeader);
        }

        // DELETE: api/PurchaseHeaders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePurchaseHeader(string id)
        {
            var purchaseHeader = await _context.PurchaseHeader.FindAsync(id);

            if(purchaseHeader.Status != DocumentStatus.Open)
            {
                return BadRequest("Cannot edit a document with status other than Open");
            }

            if (purchaseHeader == null)
            {
                return NotFound();
            }

            _context.PurchaseHeader.Remove(purchaseHeader);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PurchaseHeaderExists(string id)
        {
            return _context.PurchaseHeader.Any(e => e.Id == id);
        }

        private bool DocumentNoExists(string documentNo)
        {
            return _context.PurchaseHeader.Any(e => e.DocumentNo == documentNo);
        }
    }
}
