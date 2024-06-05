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
    public class EmployeesController : Controller
    {
        private IDbContextFactory<PinkPoloContext> _contextFactory;

        public EmployeesController(IDbContextFactory<PinkPoloContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        [HttpGet]
        public async Task<IEnumerable<Employee>> GetAll()
        {
            PinkPoloContext context = _contextFactory.CreateDbContext();
            return await context.Employee.ToListAsync();
        }

        [HttpGet("{id}", Name = "GetEmployee")]
        public async Task<Employee?> GetById(long id)
        {
            PinkPoloContext context = _contextFactory.CreateDbContext();
            return await context.Employee.FindAsync(id);
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("EmployeeID,FullName,PhoneNumber,Username,Passphrase,IsAppAllowed,IsAdmin")] Employee item)
        {
            if (ModelState.IsValid)
            {
                PinkPoloContext context = _contextFactory.CreateDbContext();
                context.Add(item);

                await context.SaveChangesAsync();
                return CreatedAtRoute("GetEmployee", new { id = item.EmployeeID }, item);
            }

            return BadRequest();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] Employee item)
        {
            if (item == null || item.EmployeeID != id)
            {
                return BadRequest();
            }

            PinkPoloContext context = _contextFactory.CreateDbContext();

            var employee = await context.Employee.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            context.Employee.Update(item);
            await context.SaveChangesAsync();

            return new NoContentResult();
        }

        [HttpPatch("{id}")]
        public IActionResult Update([FromBody] Employee item, long id)
        {
            if (item == null)
            {
                return BadRequest();
            }

            PinkPoloContext context = _contextFactory.CreateDbContext();

            var employee = context.Employee.Find(id);
            if (employee == null)
            {
                return NotFound();
            }

            item.EmployeeID = employee.EmployeeID;

            context.Employee.Update(item);
            context.SaveChanges();

            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            PinkPoloContext context = _contextFactory.CreateDbContext();

            var employee = await context.Employee.FirstOrDefaultAsync(m => m.EmployeeID == id);
            if (employee == null)
            {
                return NotFound();
            }

            context.Remove(employee);
            await context.SaveChangesAsync();

            return new NoContentResult();
        }
    }
}