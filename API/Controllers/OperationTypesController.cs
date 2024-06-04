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
    public class OperationTypesController : Controller
    {
        private IDbContextFactory<PinkPoloContext> _contextFactory;

        public OperationTypesController(IDbContextFactory<PinkPoloContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        [HttpGet]
        public async Task<IEnumerable<OperationType>> GetAll()
        {
            PinkPoloContext context = _contextFactory.CreateDbContext();
            return await context.OperationType.ToListAsync();
        }

        [HttpGet("{id}", Name = "GetOperationType")]
        public async Task<OperationType?> GetById(int id)
        {
            PinkPoloContext context = _contextFactory.CreateDbContext();
            return await context.OperationType.FindAsync(id);
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Name")] OperationType item)
        {
            if (ModelState.IsValid)
            {
                PinkPoloContext context = _contextFactory.CreateDbContext();
                context.Add(item);

                await context.SaveChangesAsync();
                return CreatedAtRoute("GetOperationType", new { id = item.OperationTypeID }, item);
            }

            return BadRequest();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] OperationType item)
        {
            if (item == null || item.OperationTypeID != id)
            {
                return BadRequest();
            }

            PinkPoloContext context = _contextFactory.CreateDbContext();

            var operationType = await context.OperationType.FindAsync(id);
            if (operationType == null)
            {
                return NotFound();
            }

            context.OperationType.Update(item);
            await context.SaveChangesAsync();

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

            var operationType = await context.OperationType.FirstOrDefaultAsync(m => m.OperationTypeID == id);
            if (operationType == null)
            {
                return NotFound();
            }

            context.Remove(operationType);
            await context.SaveChangesAsync();

            return new NoContentResult();
        }
    }
}