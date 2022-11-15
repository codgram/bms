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

namespace Application.Server.Controllers.Org
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public CompaniesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: api/Company
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Company>>> GetCompany()
        {
            var user = await _userManager.GetUserAsync(User);
            var memberCompanies = await _context.Member.Where(m => m.ApplicationUserId == user.Id).Select(m => m.CompanyId).ToArrayAsync();

            
            // Get only the companies that the user is a member of
            return await _context.Company.Where(c => memberCompanies.Contains(c.Id)).ToListAsync();
        }

        [HttpGet("get")]
        public async Task<ActionResult<string[]>> GetCompanyField([FromQuery] string field)
        {

            return await _context.Company.Select(c => c.Name).ToArrayAsync();
        }



        // GET: api/Company/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Company>> GetCompany(string id)
        {
            
            var company = await _context.Company.FindAsync(id);

            // Check if the user is a member of the company
            var user = await _userManager.GetUserAsync(User);
            var memberCompanies = await _context.Member.Where(m => m.ApplicationUserId == user.Id).Select(m => m.CompanyId).ToArrayAsync();

            if (company == null || !memberCompanies.Contains(company.Id))
            {
                return NotFound();
            }

            return company;
        }

        // PUT: api/Company/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompany(string id, Company company)
        {
            if (id != company.Id)
            {
                return BadRequest();
            }

            _context.Entry(company).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyExists(id))
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

        // POST: api/Company
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Company>> PostCompany(Company company)
        {
            _context.Company.Add(company);
            try
            {
                await _context.SaveChangesAsync();

                var user = await _userManager.GetUserAsync(User);

                // create members
                var member = new Member()
                {
                    CompanyId = company.Id,
                    ApplicationUserId = user.Id
                };

                _context.Member.Add(member);

                // create role
                var role = new IdentityRole()
                {
                    Name = $"{company.Code}Owner"
                };

                await _roleManager.CreateAsync(role);


                // add user to role
                await _userManager.AddToRoleAsync(user, role.Name);
                
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CompanyExists(company.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCompany", new { id = company.Id }, company);
        }

        // DELETE: api/Company/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(string id)
        {
            var company = await _context.Company.FindAsync(id);

            if (company == null)
            {
                return NotFound();
            }

            // delete all members of the company
            var members = await _context.Member.Where(m => m.CompanyId == id).ToListAsync();

            if(members is not null)
            {
                foreach (var member in members)
                {
                    _context.Member.Remove(member);
                    await _context.SaveChangesAsync();
                }
            }

            _context.Company.Remove(company);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CompanyExists(string id)
        {
            return _context.Company.Any(e => e.Id == id);
        }
    }
}
