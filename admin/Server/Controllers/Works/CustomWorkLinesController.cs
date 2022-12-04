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
using System.Text;

namespace Application.Server.Controllers.Works
{
    [Route("api/works/custom/lines")]
    [ApiController]
    public class CustomWorkLinesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CustomWorkLinesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/CustomWorkLine
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomWorkLine>>> GetCustomWorkLine()
        {
            var user = await _userManager.GetUserAsync(User);
            var memberCompanies = await _context.Member.Where(m => m.ApplicationUserId == user.Id).Select(m => m.CompanyId).ToArrayAsync();

            
            // Get only the companies that the user is a member of
            return await _context.CustomWorkLine.Include(c => c.CustomWorkHeader).Where(c => memberCompanies.Contains(c.CustomWorkHeader.CompanyId)).ToListAsync();
        }


        // GET: api/works/custom/lines/filter
        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<CustomWorkLine>>> GetCustomWorkLineByFilter([FromQuery] string companyId, [FromQuery] string? customWorkHeaderId)
        {
            var user = await _userManager.GetUserAsync(User);
            var memberCompanies = await _context.Member.Where(m => m.ApplicationUserId == user.Id).Select(m => m.CompanyId).ToArrayAsync();

            if(memberCompanies.Contains(companyId))
            {
                return await _context.CustomWorkLine.Include(c => c.CustomWorkHeader)
                                                            .Where(c => c.CustomWorkHeader.CompanyId == companyId)
                                                            .Where(c => !String.IsNullOrEmpty(customWorkHeaderId) ? c.CustomWorkHeaderId == customWorkHeaderId : true)
                                                            .ToListAsync();
            }
            else
            {
                return Unauthorized();
            }
            
            
        }



        // GET: api/CustomWorkLine/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomWorkLine>> GetCustomWorkLine(string id)
        {
            
            var customWorkLine = await _context.CustomWorkLine.FindAsync(id);

            // Check if the user is a member of the customWorkLine
            var user = await _userManager.GetUserAsync(User);
            var memberCustomWorkLines = await _context.Member.Where(m => m.ApplicationUserId == user.Id).Select(m => m.CompanyId).ToArrayAsync();

            if (customWorkLine == null || !memberCustomWorkLines.Contains(customWorkLine.Id))
            {
                return NotFound();
            }

            return customWorkLine;
        }

        // PUT: api/CustomWorkLine/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomWorkLine(string id, CustomWorkLine customWorkLine)
        {
            if (id != customWorkLine.Id)
            {
                return BadRequest();
            }

            _context.Entry(customWorkLine).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomWorkLineExists(id))
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

        // POST: api/CustomWorkLine
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CustomWorkLine>> PostCustomWorkLine(CustomWorkLine customWorkLine)
        {
            _context.CustomWorkLine.Add(customWorkLine);
            try
            {
                await _context.SaveChangesAsync();

            }
            catch (DbUpdateException)
            {
                if (CustomWorkLineExists(customWorkLine.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCustomWorkLine", new { id = customWorkLine.Id }, customWorkLine);
        }

        // DELETE: api/CustomWorkLine/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomWorkLine(string id)
        {
            var customWorkLine = await _context.CustomWorkLine.FindAsync(id);

            if (customWorkLine == null)
            {
                return NotFound();
            }

            _context.CustomWorkLine.Remove(customWorkLine);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpGet("export/csv")]
        public async Task<FileContentResult> ExportCustomWorkLines(string customWorkHeaderId = "")
        {
            
            var items = await _context.CustomWorkLine.Include(c => c.CustomWorkHeader).Where(c => c.CustomWorkHeaderId == customWorkHeaderId).ToListAsync();
            var customWorkHeader = await _context.CustomWorkHeader.FindAsync(items.FirstOrDefault().CustomWorkHeaderId);

            StringBuilder sb = new StringBuilder();

            // var headers = typeof(CustomWorkLine).GetProperties().Select(property => property.Name).ToArray();

            var headers = new string[] { "Barcode", "Item No", "Description", "Size", "Quantity"};

            for(int i = 0; i < headers.Length; i++)
            {
                sb.Append(headers[i] + ",");
            }
            
            sb.Append("\r\n");

            foreach (var i in items.ToArray())
            {
                sb.Append(i.BarcodeNo.ToString() + ",");
                sb.Append(i.ItemNo + ",");
                sb.Append(!String.IsNullOrEmpty(i.Description) ? i.Description.Replace(",", "") : "" + ",");
                sb.Append(i.Size+ ",");
                sb.Append(i.Quantity + ",");
                sb.Append("\r\n");
            }

            return File(Encoding.ASCII.GetBytes(sb.ToString()), "text/csv", $"{customWorkHeader.Id}-{customWorkHeader.DocumentNo}-{customWorkHeader.CreatedOn}.txt");
        }



        private bool CustomWorkLineExists(string id)
        {
            return _context.CustomWorkLine.Any(e => e.Id == id);
        }
    }





    
}
