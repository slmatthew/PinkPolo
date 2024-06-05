using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Contexts;
using API.Models;
using Asp.Versioning;
using API.Enums;

namespace API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class CompaniesController : Controller
    {
        private IDbContextFactory<PinkPoloContext> _contextFactory;

        public CompaniesController(IDbContextFactory<PinkPoloContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        /**
         * получение списка три в одном
         * может получить как весь список, так
         * и отфильтрованный по IsCarsharing
         * 
         * соответственно можно получить: все компании, только каршеринг или только кикшеринг
         */
        private async Task<IEnumerable<Company>> GetCompaniesFilter(CompanyTypeEnum filter = CompanyTypeEnum.All)
        {
            PinkPoloContext context = _contextFactory.CreateDbContext();

            return filter switch
            {
                CompanyTypeEnum.Carsharing => await context.Company.Where(c => c.IsCarsharing).ToListAsync(),
                CompanyTypeEnum.KickSharing => await context.Company.Where(c => !c.IsCarsharing).ToListAsync(),
                _ => await context.Company.ToListAsync()
            };
        }

        [HttpGet]
        public async Task<IEnumerable<Company>> GetAll()
        {
            return await GetCompaniesFilter();
        }

        [HttpGet("{id}", Name = "GetCompany")]
        public async Task<Company?> GetById(long id)
        {
            PinkPoloContext context = _contextFactory.CreateDbContext();
            return await context.Company.FindAsync(id);
        }

        [HttpGet("type/{type}")]
        public async Task<IEnumerable<Company>> GetByType(string type)
        {
            CompanyTypeEnum filter = type switch
            {
                "carsharing" => CompanyTypeEnum.Carsharing,
                "kicksharing" => CompanyTypeEnum.KickSharing,
                _ => CompanyTypeEnum.All
            };

            if (filter == CompanyTypeEnum.All)
                BadRequest();

            return await GetCompaniesFilter(filter);
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("CompanyID,Name,IsCarsharing")] Company item)
        {
            if (ModelState.IsValid)
            {
                PinkPoloContext context = _contextFactory.CreateDbContext();
                context.Add(item);

                await context.SaveChangesAsync();
                return CreatedAtRoute("GetCompany", new { id = item.CompanyID }, item);
            }

            return BadRequest();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] Company item)
        {
            if (item == null || item.CompanyID != id)
            {
                return BadRequest();
            }

            PinkPoloContext context = _contextFactory.CreateDbContext();

            var company = await context.Company.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }

            context.Company.Update(item);
            await context.SaveChangesAsync();

            return new NoContentResult();
        }

        [HttpPatch("{id}")]
        public IActionResult Update([FromBody] Company item, long id)
        {
            if (item == null)
            {
                return BadRequest();
            }

            PinkPoloContext context = _contextFactory.CreateDbContext();

            var company = context.Company.Find(id);
            if (company == null)
            {
                return NotFound();
            }

            item.CompanyID = company.CompanyID;

            context.Company.Update(item);
            context.SaveChanges();

            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Int64? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            PinkPoloContext context = _contextFactory.CreateDbContext();

            var company = await context.Company.FirstOrDefaultAsync(m => m.CompanyID == id);
            if (company == null)
            {
                return NotFound();
            }

            context.Remove(company);
            await context.SaveChangesAsync();

            return new NoContentResult();
        }
    }
}
