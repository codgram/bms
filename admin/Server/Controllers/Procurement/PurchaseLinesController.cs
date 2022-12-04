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
    [Route("api/purchase/lines")]
    [ApiController]
    public class PurchaseLinesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PurchaseLinesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/PurchaseLines
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PurchaseLine>>> GetPurchaseLines([FromQuery] string companyId, [FromQuery] string purchaseHeaderId)
        {
            return await _context.PurchaseLine.Include(p => p.Item).Include(p => p.PurchaseHeader)
                                                .Where(x => x.PurchaseHeaderId == purchaseHeaderId && x.PurchaseHeader.CompanyId == companyId)
                                                .ToListAsync();
        }

        // GET: api/PurchaseLines/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PurchaseLine>> GetPurchaseLine(string id, string? item)
        {
            var purchaseLine = await _context.PurchaseLine.Include(p => p.Item).FirstOrDefaultAsync(p => p.Id == id);

            if (purchaseLine == null)
            {
                return NotFound();
            }

            return purchaseLine;
        }

        // PUT: api/PurchaseLines/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPurchaseLine(string id, PurchaseLine purchaseLine)
        {

            if(purchaseLine.PurchaseHeader.Status != DocumentStatus.Open)
            {
                return BadRequest("Cannot edit a document with status other than Open");
            }

            if (id != purchaseLine.Id)
            {
                return BadRequest();
            }

            _context.Entry(purchaseLine).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PurchaseLineExists(id))
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

        // POST: api/PurchaseLines
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PurchaseLine>> PostPurchaseLine(PurchaseLineDetail purchaseLineDetail, [FromQuery] string companyId)
        {
            
            var purchaseHeader = await _context.PurchaseHeader.FindAsync(purchaseLineDetail.PurchaseHeaderId);

            if(purchaseHeader.Status != DocumentStatus.Open)
            {
                return BadRequest("Cannot edit a document with status other than Open");
            }

            if(purchaseLineDetail.ItemCode == null)
            {
                return BadRequest("Item is required");
            }

            if(!ItemCodeExists(companyId, purchaseLineDetail.ItemCode))
            {
                return BadRequest("Item does not exist");
            }

            var item = await _context.Item.FirstOrDefaultAsync(i => i.CompanyId == companyId && i.Code == purchaseLineDetail.ItemCode);

            var purchaseLine = new PurchaseLine
            {
                PurchaseHeaderId = purchaseLineDetail.PurchaseHeaderId,
                ItemId = item.Id,
                OrderedQuantity = purchaseLineDetail.OrderedQuantity,
                UnitPrice = purchaseLineDetail.UnitPrice,
            };

            _context.PurchaseLine.Add(purchaseLine);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPurchaseLine", new { id = purchaseLine.Id, item = item }, purchaseLine);
        }

        // DELETE: api/PurchaseLines/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePurchaseLine(string id)
        {
            var purchaseLine = await _context.PurchaseLine.Include(p => p.PurchaseHeader).FirstOrDefaultAsync(p => p.Id == id);

            if(purchaseLine.PurchaseHeader.Status != DocumentStatus.Open)
            {
                return BadRequest("Cannot edit a document with status other than Open");
            }

            if (purchaseLine == null)
            {
                return NotFound();
            }

            _context.PurchaseLine.Remove(purchaseLine);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PurchaseLineExists(string id)
        {
            return _context.PurchaseLine.Any(e => e.Id == id);
        }

        private bool ItemCodeExists(string companyId, string code)
        {
            return _context.Item.Any(e => e.CompanyId == companyId && e.Code == code);
        }
    }
}
