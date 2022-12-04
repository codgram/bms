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

namespace Application.Server.Controllers.Works
{
    [Route("api/works/custom/headers")]
    [ApiController]
    public class CustomWorkHeadersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CustomWorkHeadersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/CustomWorkHeader
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomWorkHeader>>> GetCustomWorkHeader()
        {
            var user = await _userManager.GetUserAsync(User);
            var memberCompanies = await _context.Member.Where(m => m.ApplicationUserId == user.Id).Select(m => m.CompanyId).ToArrayAsync();

            
            // Get only the companies that the user is a member of
            return await _context.CustomWorkHeader.Where(c => memberCompanies.Contains(c.CompanyId)).ToListAsync();
        }



        // GET: api/CustomWorkHeader/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomWorkHeader>> GetCustomWorkHeader(string id)
        {
            
            var customWorkHeader = await _context.CustomWorkHeader.FindAsync(id);

            // Check if the user is a member of the customWorkHeader
            var user = await _userManager.GetUserAsync(User);
            var memberCompanies = await _context.Member.Where(m => m.ApplicationUserId == user.Id).Select(m => m.CompanyId).ToArrayAsync();

            if (customWorkHeader == null || !memberCompanies.Contains(customWorkHeader.CompanyId))
            {
                return Unauthorized();
            }

            return customWorkHeader;
        }

        // PUT: api/CustomWorkHeader/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomWorkHeader(string id, CustomWorkHeader customWorkHeader)
        {
            if (id != customWorkHeader.Id)
            {
                return BadRequest();
            }

            _context.Entry(customWorkHeader).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomWorkHeaderExists(id))
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

        // POST: api/CustomWorkHeader
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CustomWorkHeader>> PostCustomWorkHeader(CustomWorkHeader customWorkHeader)
        {

            if(DocumentNoExists(customWorkHeader.DocumentNo, customWorkHeader.CompanyId))
            {
                return BadRequest("Document No already exists");
            }

            _context.CustomWorkHeader.Add(customWorkHeader);
            try
            {
                await _context.SaveChangesAsync();

            }
            catch (DbUpdateException)
            {
                if (CustomWorkHeaderExists(customWorkHeader.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCustomWorkHeader", new { id = customWorkHeader.Id }, customWorkHeader);
        }

        // DELETE: api/CustomWorkHeader/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomWorkHeader(string id)
        {
            var customWorkHeader = await _context.CustomWorkHeader.FindAsync(id);

            if (customWorkHeader == null)
            {
                return NotFound();
            }

            _context.CustomWorkHeader.Remove(customWorkHeader);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CustomWorkHeaderExists(string id)
        {
            return _context.CustomWorkHeader.Any(e => e.Id == id);
        }

        private bool DocumentNoExists(string documentNo, string companyId)
        {
            return _context.CustomWorkHeader.Any(e => e.DocumentNo == documentNo && e.CompanyId == companyId);
        }
    }
}
