using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using API.Contexts;
using API.Models;
using Asp.Versioning;

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

        [HttpGet]
        public async Task<IEnumerable<Company>> GetAll()
        {
            PinkPoloContext context = _contextFactory.CreateDbContext();
            return await context.Company.ToListAsync();
        }
    }
}
