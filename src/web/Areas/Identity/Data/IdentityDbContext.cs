using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace web.Areas.Identity.Data;

public class IdentityDbContext : IdentityDbContext<IdentityUser>
{
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
                base.OnModelCreating(builder);
        }
}
