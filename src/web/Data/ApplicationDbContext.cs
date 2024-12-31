using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using web.features.artist;
using web.features.shared.domain;

namespace web.data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
{
        public DbSet<Artist> Artists { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
                base.OnModelCreating(builder);

                builder.Ignore<AggreggateRoot>();
                builder.Ignore<ValueObject>();

                builder.Entity<Artist>()
                        .HasKey(a => a.ArtistId);
                builder.Entity<Artist>()
                        .Property(a => a.ArtistId)
                        .HasConversion(id => id.Value, guid => new ArtistId(guid));
                builder.Entity<Artist>()
                        .HasOne<IdentityUser>()
                        .WithOne()
                        .HasForeignKey<Artist>(a => a.OwnerId);
        }
}
