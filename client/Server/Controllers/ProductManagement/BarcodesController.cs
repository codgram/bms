#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Application.Model.Org;
using Application.Server.Data;
using Microsoft.AspNetCore.Identity;
using Application.Model.Work;
using Application.Model.ProductManagement;

namespace Application.Server.Controllers.Org
{
    [Route("api/[controller]")]
    [ApiController]
    public class BarcodesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BarcodesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Barcode
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Barcode>>> GetBarcode()
        {
            var user = await _userManager.GetUserAsync(User);
            var memberCompanies = await _context.Member.Where(m => m.ApplicationUserId == user.Id).Select(m => m.CompanyId).ToArrayAsync();

            
            // Get only the companies that the user is a member of
            return await _context.Barcode.Where(c => memberCompanies.Contains(c.CompanyId)).ToListAsync();
        }

        // GET: api/Barcode
        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<Barcode>>> GetBarcodesByFilter([FromQuery] string companyId)
        {
            var user = await _userManager.GetUserAsync(User);

            var userIsMember = await _context.Member.AnyAsync(m => m.CompanyId == companyId && m.ApplicationUserId == user.Id);

            if(userIsMember)
            {
                return await _context.Barcode.Where(c => c.CompanyId == companyId).ToListAsync();
            }
            else
            {
                return NotFound();
            }

        }



        // GET: api/Barcode/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Barcode>> GetBarcode(string id)
        {
            
            var barcode = await _context.Barcode.FindAsync(id);

            // Check if the user is a member of the barcode
            var user = await _userManager.GetUserAsync(User);
            var memberBarcodes = await _context.Member.Where(m => m.ApplicationUserId == user.Id).Select(m => m.CompanyId).ToArrayAsync();

            if (barcode == null || !memberBarcodes.Contains(barcode.Id))
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
            try
            {
                await _context.SaveChangesAsync();

            }
            catch (DbUpdateException)
            {
                if (BarcodeExists(barcode.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetBarcode", new { id = barcode.Id }, barcode);
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
    }
}
