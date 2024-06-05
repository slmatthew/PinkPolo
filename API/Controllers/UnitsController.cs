using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Contexts;
using API.Models;
using Asp.Versioning;

namespace API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UnitsController : Controller
    {
        private IDbContextFactory<PinkPoloContext> _contextFactory;

        public UnitsController(IDbContextFactory<PinkPoloContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        [HttpGet]
        public async Task<IEnumerable<Unit>> GetAll()
        {
            PinkPoloContext context = _contextFactory.CreateDbContext();
            return await context.Unit.ToListAsync();
        }

        [HttpGet("{id}", Name = "GetUnit")]
        public async Task<Unit?> GetById(int id)
        {
            PinkPoloContext context = _contextFactory.CreateDbContext();
            return await context.Unit.FindAsync(id);
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("CompanyID,IsACtive,Lat,Lng,UniqueNumber")] Unit item)
        {
            if (ModelState.IsValid)
            {
                PinkPoloContext context = _contextFactory.CreateDbContext();
                context.Add(item);

                await context.SaveChangesAsync();
                return CreatedAtRoute("GetUnit", new { id = item.UnitID }, item);
            }

            return BadRequest();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Unit item)
        {
            if (item == null || item.UnitID != id)
            {
                return BadRequest();
            }

            PinkPoloContext context = _contextFactory.CreateDbContext();

            var unit = await context.Unit.FindAsync(id);
            if (unit == null)
            {
                return NotFound();
            }

            context.Unit.Update(item);
            await context.SaveChangesAsync();

            return new NoContentResult();
        }

        [HttpPatch("{id}")]
        public IActionResult Update([FromBody] Unit item, int id)
        {
            if (item == null)
            {
                return BadRequest();
            }

            PinkPoloContext context = _contextFactory.CreateDbContext();

            var unit = context.Unit.Find(id);
            if (unit == null)
            {
                return NotFound();
            }

            item.UnitID = unit.UnitID;

            context.Unit.Update(item);
            context.SaveChanges();

            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            PinkPoloContext context = _contextFactory.CreateDbContext();

            var unit = await context.Unit.FirstOrDefaultAsync(m => m.UnitID == id);
            if (unit == null)
            {
                return NotFound();
            }

            context.Remove(unit);
            await context.SaveChangesAsync();

            return new NoContentResult();
        }
    }
}