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
    public class CurrencyExchangesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CurrencyExchangesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/CurrencyExchanges
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CurrencyExchange>>> GetCurrencyExchanges([FromQuery] string companyId)
        {
            return await _context.CurrencyExchange.Where(x => x.CompanyId == companyId).ToListAsync();
        }

        // GET: api/CurrencyExchanges/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CurrencyExchange>> GetCurrencyExchange(string id)
        {
            var currencyExchange = await _context.CurrencyExchange.FindAsync(id);

            if (currencyExchange == null)
            {
                return NotFound();
            }

            return currencyExchange;
        }

        // PUT: api/CurrencyExchanges/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCurrencyExchange(string id, CurrencyExchange currencyExchange)
        {
            if (id != currencyExchange.Id)
            {
                return BadRequest();
            }

            _context.Entry(currencyExchange).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CurrencyExchangeExists(id))
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

        // POST: api/CurrencyExchanges
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CurrencyExchange>> PostCurrencyExchange(CurrencyExchange currencyExchange)
        {
            _context.CurrencyExchange.Add(currencyExchange);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCurrencyExchange", new { id = currencyExchange.Id }, currencyExchange);
        }

        // DELETE: api/CurrencyExchanges/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCurrencyExchange(string id)
        {
            var currencyExchange = await _context.CurrencyExchange.FindAsync(id);
            if (currencyExchange == null)
            {
                return NotFound();
            }

            _context.CurrencyExchange.Remove(currencyExchange);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CurrencyExchangeExists(string id)
        {
            return _context.CurrencyExchange.Any(e => e.Id == id);
        }
    }
}
