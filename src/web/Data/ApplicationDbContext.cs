using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using web.Features.Artists;
using web.Features.ArtPieces;
using web.Features.Reviews;
using web.Features.Shared.domain;

namespace web.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>
{
        public required DbSet<Artist> Artists { get; set; }
        public required DbSet<ArtPiece> ArtPieces { get; set; }
        public required DbSet<Review> Reviews { get; set; }

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

                builder.Entity<Review>()
                        .HasKey(r => r.Id);
                builder.Entity<Review>()
                        .Property(r => r.Id)
                        .HasConversion(id => id.Value, guid => new ReviewId(guid));
                builder.Entity<Review>()
                        .Property(r => r.Date)
                        .IsRequired();
                builder.Entity<Review>()
                        .HasOne<IdentityUser<Guid>>()
                        .WithMany()
                        .HasForeignKey(r => r.ReviewerId);
                builder.Entity<Review>()
                        .HasOne<ArtPiece>()
                        .WithMany()
                        .HasForeignKey(r => r.ArtPieceId);
        }
}
