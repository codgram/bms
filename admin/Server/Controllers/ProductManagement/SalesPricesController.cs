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
    public class SalesPricesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SalesPricesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/SalesPrices
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SalesPrice>>> GetSalesPrices([FromQuery] string itemId)
        {
            return await _context.SalesPrice.Where(x => x.ItemId == itemId).ToListAsync();
        }

        // GET: api/SalesPrices/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SalesPrice>> GetSalesPrice(string id)
        {
            var salesPrice = await _context.SalesPrice.FindAsync(id);

            if (salesPrice == null)
            {
                return NotFound();
            }

            return salesPrice;
        }

        // PUT: api/SalesPrices/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSalesPrice(string id, SalesPrice salesPrice)
        {
            if (id != salesPrice.Id)
            {
                return BadRequest();
            }

            _context.Entry(salesPrice).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SalesPriceExists(id))
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

        // POST: api/SalesPrices
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SalesPrice>> PostSalesPrice(SalesPrice salesPrice)
        {
            if(ItemSalesPriceExists(salesPrice.ItemId, salesPrice.Price))
            {
                salesPrice = _context.SalesPrice.Where(x => x.ItemId == salesPrice.ItemId && x.Price == salesPrice.Price && x.Status == LineStatus.Active).FirstOrDefault();
                await PutSalesPrice(salesPrice.Id, salesPrice);
            }
            
            else
            {
                // close the old sales price of the item
                var oldSalesPrice = _context.SalesPrice.Where(x => x.ItemId == salesPrice.ItemId && x.Status == LineStatus.Active).FirstOrDefault();

                if(oldSalesPrice is not null)
                {
                    oldSalesPrice.Status = LineStatus.Inactive;
                    oldSalesPrice.EndDate = DateTime.Now;

                    await PutSalesPrice(oldSalesPrice.Id, oldSalesPrice);
                }
                


                // create the new sales price of the item
                salesPrice.StartDate = DateTime.Now.AddDays(1);
                _context.SalesPrice.Add(salesPrice);
                await _context.SaveChangesAsync();
            }


            return CreatedAtAction("GetSalesPrice", new { id = salesPrice.Id }, salesPrice);
        }

        // DELETE: api/SalesPrices/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSalesPrice(string id)
        {
            var salesPrice = await _context.SalesPrice.FindAsync(id);
            if (salesPrice == null)
            {
                return NotFound();
            }

            _context.SalesPrice.Remove(salesPrice);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SalesPriceExists(string id)
        {
            return _context.SalesPrice.Any(e => e.Id == id);
        }
        private bool ItemSalesPriceExists(string itemId, decimal price)
        {
            return _context.SalesPrice.Any(e => e.ItemId == itemId && e.Status == LineStatus.Active && e.Price == price);
        }
    }
}
