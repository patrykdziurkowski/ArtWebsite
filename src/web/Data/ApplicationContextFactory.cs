using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace web.Data;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
        public ApplicationDbContext CreateDbContext(string[] args)
        {
                DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder = new();
                optionsBuilder.UseSqlServer();
                return new ApplicationDbContext(optionsBuilder.Options);
        }
}
