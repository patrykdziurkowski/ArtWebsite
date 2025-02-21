using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using web.Features.Artists;
using web.Features.ArtPieces;
using web.Features.Reviewers;
using web.Features.Reviews;
using web.Features.Shared.domain;

namespace web.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>(options)
{
        public required DbSet<Artist> Artists { get; set; }
        public required DbSet<ArtPiece> ArtPieces { get; set; }
        public required DbSet<Review> Reviews { get; set; }
        public required DbSet<Reviewer> Reviewers { get; set; }
        public required DbSet<Like> Likes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
                base.OnModelCreating(builder);

                builder.Ignore<AggreggateRoot>();
                builder.Ignore<ValueObject>();

                builder.Entity<Artist>()
                        .HasKey(a => a.Id);
                builder.Entity<Artist>()
                        .Property(a => a.Id)
                        .HasConversion(id => id.Value, guid => new ArtistId { Value = guid });
                builder.Entity<Artist>()
                        .HasOne<IdentityUser<Guid>>()
                        .WithOne()
                        .HasForeignKey<Artist>(a => a.UserId);

                builder.Entity<ArtPiece>()
                        .HasKey(a => a.Id);
                builder.Entity<ArtPiece>()
                        .Property(a => a.Id)
                        .HasConversion(id => id.Value, guid => new ArtPieceId { Value = guid });
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
                        .HasConversion(id => id.Value, guid => new ReviewId { Value = guid });
                builder.Entity<Review>()
                        .Property(r => r.Date)
                        .IsRequired();
                builder.Entity<Review>()
                        .HasOne<Reviewer>()
                        .WithMany()
                        .HasForeignKey(r => r.ReviewerId);
                builder.Entity<Review>()
                        .HasOne<ArtPiece>()
                        .WithMany()
                        .HasForeignKey(r => r.ArtPieceId);

                builder.Entity<Reviewer>()
                        .HasKey(r => r.Id);
                builder.Entity<Reviewer>()
                       .Property(r => r.Id)
                       .HasConversion(id => id.Value, guid => new ReviewerId { Value = guid });
                builder.Entity<Reviewer>()
                        .Property(r => r.Name)
                        .IsRequired();
                builder.Entity<Reviewer>()
                        .Property(r => r.JoinDate)
                        .IsRequired();
                builder.Entity<Reviewer>()
                        .Ignore(r => r.ReviewCount);
                builder.Entity<Reviewer>()
                        .HasOne<IdentityUser<Guid>>()
                        .WithOne()
                        .HasForeignKey<Reviewer>(r => r.UserId);
                builder.Entity<Reviewer>()
                        .HasMany(l => l.ActiveLikes)
                        .WithOne()
                        .HasForeignKey(l => l.ReviewerId);

                builder.Entity<Like>()
                        .HasKey(l => l.Id);
                builder.Entity<Like>()
                       .Property(l => l.Id)
                       .HasConversion(id => id.Value, guid => new LikeId { Value = guid });
                builder.Entity<Like>()
                        .Property(l => l.Date)
                        .IsRequired();
                builder.Entity<Like>()
                        .Property(l => l.ExpirationDate)
                        .IsRequired();
                builder.Entity<Like>()
                        .HasOne<ArtPiece>()
                        .WithMany()
                        .HasForeignKey(l => l.ArtPieceId);
        }
}
