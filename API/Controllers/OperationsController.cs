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
    public class OperationsController : Controller
    {
        private IDbContextFactory<PinkPoloContext> _contextFactory;

        public OperationsController(IDbContextFactory<PinkPoloContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        [HttpGet]
        public async Task<IEnumerable<Operation>> GetAll()
        {
            PinkPoloContext context = _contextFactory.CreateDbContext();
            return await context.Operation.ToListAsync();
        }

        [HttpGet("{id}", Name = "GetOperation")]
        public async Task<Operation?> GetById(int id)
        {
            PinkPoloContext context = _contextFactory.CreateDbContext();
            return await context.Operation.FindAsync(id);
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("EmployeeID,UnitID,OperationType,IssuedAt")] Operation item)
        {
            if (ModelState.IsValid)
            {
                PinkPoloContext context = _contextFactory.CreateDbContext();
                context.Add(item);

                await context.SaveChangesAsync();
                return CreatedAtRoute("GetOperation", new { id = item.OperationID }, item);
            }

            return BadRequest();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Operation item)
        {
            if (item == null || item.OperationID != id)
            {
                return BadRequest();
            }

            PinkPoloContext context = _contextFactory.CreateDbContext();

            var operation = await context.Operation.FindAsync(id);
            if (operation == null)
            {
                return NotFound();
            }

            context.Operation.Update(item);
            await context.SaveChangesAsync();

            return new NoContentResult();
        }

        [HttpPatch("{id}")]
        public IActionResult Update([FromBody] Operation item, int id)
        {
            if (item == null)
            {
                return BadRequest();
            }

            PinkPoloContext context = _contextFactory.CreateDbContext();

            var operation = context.Operation.Find(id);
            if (operation == null)
            {
                return NotFound();
            }

            item.OperationID = operation.OperationID;

            context.Operation.Update(item);
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

            var operation = await context.Operation.FirstOrDefaultAsync(m => m.OperationID == id);
            if (operation == null)
            {
                return NotFound();
            }

            context.Remove(operation);
            await context.SaveChangesAsync();

            return new NoContentResult();
        }
    }
}