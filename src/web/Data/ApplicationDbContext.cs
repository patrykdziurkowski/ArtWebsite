using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using web.features.artist;
using web.features.shared.domain;
using web.Features.ArtPiece;

namespace web.data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>
{
        public DbSet<Artist> Artists { get; set; } = null!;
        public DbSet<ArtPiece> ArtPieces { get; set; } = null!;

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
                        .HasKey(a => a.Id);
                builder.Entity<Artist>()
                        .Property(a => a.Id)
                        .HasConversion(id => id.Value, guid => new ArtistId(guid));
                builder.Entity<Artist>()
                        .HasOne<IdentityUser<Guid>>()
                        .WithOne()
                        .HasForeignKey<Artist>(a => a.OwnerId);

                builder.Entity<ArtPiece>()
                        .HasKey(a => a.Id);
                builder.Entity<ArtPiece>()
                        .Property(a => a.Id)
                        .HasConversion(id => id.Value, guid => new ArtPieceId(guid));
                builder.Entity<ArtPiece>()
                        .HasOne<Artist>()
                        .WithMany()
                        .HasForeignKey(a => a.ArtistId);
                builder.Entity<ArtPiece>()
                        .Property(a => a.ImagePath)
                        .IsRequired();
                builder.Entity<ArtPiece>()
                        .Property(a => a.UploadDate)
                        .IsRequired();
        }
}
