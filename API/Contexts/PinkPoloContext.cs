using Microsoft.EntityFrameworkCore;
using API.Models;

namespace API.Contexts
{
    public class PinkPoloContext : DbContext
    {
        public DbSet<Company> Company { get; set; }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<Operation> Operation { get; set; }
        public DbSet<OperationType> OperationType { get; set; }
        public DbSet<Unit> Unit { get; set; }

        public PinkPoloContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
