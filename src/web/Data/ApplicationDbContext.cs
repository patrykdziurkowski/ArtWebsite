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
        public DbSet<Artist> Artists { get; set; }
        public DbSet<ArtPiece> ArtPieces { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Reviewer> Reviewers { get; set; }
        public DbSet<Like> Likes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
                base.OnModelCreating(builder);

                builder.Ignore<AggreggateRoot>();
                builder.Ignore<ValueObject>();

                var artist = builder.Entity<Artist>();
                artist.HasKey(a => a.Id);
                artist.Property(a => a.Id)
                        .HasConversion(id => id.Value, guid => new ArtistId { Value = guid });
                artist.HasOne<IdentityUser<Guid>>()
                        .WithOne()
                        .HasForeignKey<Artist>(a => a.UserId);

                var boost = builder.Entity<Boost>();
                boost.HasKey(b => b.Id);
                boost.Property(b => b.Id)
                        .HasConversion(id => id.Value, guid => new BoostId { Value = guid });
                boost.Property(b => b.Date)
                        .IsRequired();
                boost.Property(b => b.ExpirationDate)
                        .IsRequired();
                boost.HasOne<ArtPiece>()
                        .WithMany()
                        .HasForeignKey(b => b.ArtPieceId);

                var artPiece = builder.Entity<ArtPiece>();
                artPiece.HasKey(a => a.Id);
                artPiece.Property(a => a.Id)
                        .HasConversion(id => id.Value, guid => new ArtPieceId { Value = guid });
                artPiece.HasOne<Artist>()
                        .WithMany()
                        .HasForeignKey(a => a.ArtistId);
                artPiece.Property(a => a.ImagePath)
                        .IsRequired();
                artPiece.Ignore(a => a.AverageRating);
                artPiece.Property(a => a.UploadDate)
                        .IsRequired();

                var review = builder.Entity<Review>();
                review.HasKey(r => r.Id);
                review.Property(r => r.Id)
                        .HasConversion(id => id.Value, guid => new ReviewId { Value = guid });
                review.Property(r => r.Date)
                        .IsRequired();
                review.Property(r => r.Rating)
                        .HasConversion(rating => rating.Value, value => new Rating(value));
                review.HasOne<Reviewer>()
                        .WithMany()
                        .HasForeignKey(r => r.ReviewerId);
                review.HasOne<ArtPiece>()
                        .WithMany()
                        .HasForeignKey(r => r.ArtPieceId);

                var reviewer = builder.Entity<Reviewer>();
                reviewer.HasKey(r => r.Id);
                reviewer.Property(r => r.Id)
                       .HasConversion(id => id.Value, guid => new ReviewerId { Value = guid });
                reviewer.Property(r => r.Name)
                        .IsRequired();
                reviewer.Property(r => r.JoinDate)
                        .IsRequired();
                reviewer.Ignore(r => r.ReviewCount);
                reviewer.HasOne<IdentityUser<Guid>>()
                        .WithOne()
                        .HasForeignKey<Reviewer>(r => r.UserId);
                reviewer.HasMany(l => l.ActiveLikes)
                        .WithOne()
                        .HasForeignKey(l => l.ReviewerId);

                var like = builder.Entity<Like>();
                like.HasKey(l => l.Id);
                like.Property(l => l.Id)
                       .HasConversion(id => id.Value, guid => new LikeId { Value = guid });
                like.Property(l => l.Date)
                        .IsRequired();
                like.Property(l => l.ExpirationDate)
                        .IsRequired();
                like.HasOne<ArtPiece>()
                        .WithMany()
                        .HasForeignKey(l => l.ArtPieceId);
        }
}
