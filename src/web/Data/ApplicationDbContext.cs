using System.Data;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using web.Features.Artists;
using web.Features.ArtPieces;
using web.Features.Browse;
using web.Features.Leaderboard.Artist;
using web.Features.Leaderboard.Reviewer;
using web.Features.Missions;
using web.Features.Reviewers;
using web.Features.Reviews;
using web.Features.Shared.domain;
using web.Features.Suspensions;
using web.Features.Tags;

namespace web.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>(options)
{
        public DbSet<Suspension> Suspensions { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<ArtPiece> ArtPieces { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Reviewer> Reviewers { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Boost> Boosts { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ArtPieceTag> ArtPieceTags { get; set; }
        public DbSet<ReviewerPointAward> ReviewerPointAwards { get; set; }
        public DbSet<ArtistPointAward> ArtistPointAwards { get; set; }
        public DbSet<MissionProgress> MissionProgresses { get; set; }
        public DbSet<ArtPieceServed> ArtPiecesServed { get; set; }

        /// <summary>
        /// This is a custom method mapped to an SQL Server function used for generating
        /// values between 0 and 1 to be used in queries. Used instead of EF.Functions.Random
        /// method due to how that method gets translated for SQL Server, which causes the
        /// same, deterministic results between queries.
        /// 
        /// Made for use inside IQueryable expressions.
        /// </summary>
        /// <returns>A random value between 0 and 1</returns>
        public double Random() => throw new NotSupportedException(); // the method's body doesn't matter here

        protected override void OnModelCreating(ModelBuilder builder)
        {
                base.OnModelCreating(builder);

                // Custom SQL Server translation for our custom Random implementation used
                // due to RAND() being determinsitic between query calls. NEWID() and CHECKSUM()
                // inside here allow us to achieve actual per-row per-query randomness.
                //
                // Generates a value between 0 and 1.
                builder.HasDbFunction(GetType().GetMethod(nameof(Random))!)
                        .HasTranslation(args =>
                        {
                                var intMapping = new IntTypeMapping("int", DbType.Int32);
                                var doubleMapping = new DoubleTypeMapping("float", DbType.Double);
                                var guidMapping = new GuidTypeMapping("uniqueidentifier", DbType.Guid);
                                var binary16Mapping = new ByteArrayTypeMapping("binary(16)");

                                var newIdExpr = new SqlFunctionExpression(
                                        "NEWID",
                                        [],
                                        false,
                                        [],
                                        typeof(Guid),
                                        guidMapping);

                                var castToBinaryExpr = new SqlUnaryExpression(
                                        ExpressionType.Convert,
                                        newIdExpr,
                                        typeof(byte[]),
                                        binary16Mapping);

                                var checksumExpr = new SqlFunctionExpression(
                                        "CHECKSUM",
                                        [castToBinaryExpr],
                                        true,
                                        [true],
                                        typeof(int),
                                        intMapping);

                                var absExpr = new SqlFunctionExpression(
                                        "ABS",
                                        [checksumExpr],
                                        true,
                                        [true],
                                        typeof(int),
                                        intMapping);

                                var modExpr = new SqlBinaryExpression(
                                        ExpressionType.Modulo,
                                        absExpr,
                                        new SqlConstantExpression(10000, intMapping),
                                        typeof(int),
                                        intMapping);

                                var divideExpr = new SqlBinaryExpression(
                                        ExpressionType.Divide,
                                        modExpr,
                                        new SqlConstantExpression(10000.0, doubleMapping),
                                        typeof(double),
                                        doubleMapping);

                                return divideExpr;
                        });

                builder.Ignore<AggregateRoot>();
                builder.Ignore<ValueObject>();

                var suspension = builder.Entity<Suspension>();
                suspension.HasKey(s => s.Id);
                suspension.Property(s => s.Id)
                        .HasConversion(id => id.Value, guid => new SuspensionId { Value = guid });
                suspension.HasOne<IdentityUser<Guid>>()
                        .WithMany()
                        .HasForeignKey(s => s.UserId)
                        .OnDelete(DeleteBehavior.Restrict);
                suspension.HasOne<IdentityUser<Guid>>()
                        .WithMany()
                        .HasForeignKey(s => s.IssuingUserId)
                        .OnDelete(DeleteBehavior.Restrict);

                var artist = builder.Entity<Artist>();
                artist.HasKey(a => a.Id);
                artist.Property(a => a.Id)
                        .HasConversion(id => id.Value, guid => new ArtistId { Value = guid });
                artist.HasOne<IdentityUser<Guid>>()
                        .WithOne()
                        .HasForeignKey<Artist>(a => a.UserId);
                artist.HasMany<Boost>()
                        .WithOne()
                        .HasForeignKey(b => b.ArtistId)
                        .OnDelete(DeleteBehavior.Cascade);

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
                        .HasForeignKey(b => b.ArtPieceId)
                        .OnDelete(DeleteBehavior.SetNull);

                var artPiece = builder.Entity<ArtPiece>();
                artPiece.HasKey(a => a.Id);
                artPiece.Property(a => a.Id)
                        .HasConversion(id => id.Value, guid => new ArtPieceId { Value = guid });
                artPiece.HasOne<Artist>()
                        .WithMany()
                        .HasForeignKey(a => a.ArtistId)
                        .OnDelete(DeleteBehavior.Restrict);
                artPiece.Property(a => a.ImagePath)
                        .IsRequired();
                artPiece.Property(a => a.UploadDate)
                        .IsRequired();
                artPiece.Property(r => r.AverageRating)
                        .HasConversion(rating => rating.Value, value => value == 0 ? Rating.Empty : new Rating(value));

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
                        .HasForeignKey(r => r.ReviewerId)
                        .OnDelete(DeleteBehavior.Restrict);
                review.HasOne<ArtPiece>()
                        .WithMany()
                        .HasForeignKey(r => r.ArtPieceId)
                        .OnDelete(DeleteBehavior.Cascade);

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
                        .HasForeignKey<Reviewer>(r => r.UserId)
                        .OnDelete(DeleteBehavior.Restrict);
                reviewer.HasMany(l => l.ActiveLikes)
                        .WithOne()
                        .HasForeignKey(l => l.ReviewerId)
                        .OnDelete(DeleteBehavior.Restrict);

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
                        .HasForeignKey(l => l.ArtPieceId)
                        .OnDelete(DeleteBehavior.Cascade);
                like.HasOne<Review>()
                        .WithOne()
                        .HasForeignKey<Like>("ReviewId")
                        .OnDelete(DeleteBehavior.Restrict);

                var tag = builder.Entity<Tag>();
                tag.HasKey(t => t.Id);
                tag.Property(t => t.Id)
                       .HasConversion(id => id.Value, guid => new TagId { Value = guid });
                tag.Property(t => t.Name)
                        .IsRequired();
                tag.HasMany<ArtPieceTag>()
                        .WithOne()
                        .HasForeignKey(apt => apt.TagId)
                        .OnDelete(DeleteBehavior.Cascade);

                var artPieceTag = builder.Entity<ArtPieceTag>();
                artPieceTag.HasKey(apt => apt.Id);
                artPieceTag.Property(apt => apt.Id)
                        .HasConversion(id => id.Value, guid => new ArtPieceTagId { Value = guid });
                artPieceTag.HasOne<ArtPiece>()
                        .WithMany()
                        .HasForeignKey(apt => apt.ArtPieceId)
                        .OnDelete(DeleteBehavior.Cascade);

                var reviewerPointAward = builder.Entity<ReviewerPointAward>();
                reviewerPointAward.HasKey(pa => pa.Id);
                reviewerPointAward.Property(pa => pa.Id)
                        .HasConversion(id => id.Value, guid => new ReviewerPointAwardId { Value = guid });
                reviewerPointAward.Property(pa => pa.DateAwarded)
                        .IsRequired();
                reviewerPointAward.HasOne<Reviewer>()
                        .WithMany()
                        .HasForeignKey(pa => pa.ReviewerId)
                        .OnDelete(DeleteBehavior.Cascade);

                var artistPointAward = builder.Entity<ArtistPointAward>();
                artistPointAward.HasKey(pa => pa.Id);
                artistPointAward.Property(pa => pa.Id)
                        .HasConversion(id => id.Value, guid => new ArtistPointAwardId { Value = guid });
                artistPointAward.Property(pa => pa.DateAwarded)
                        .IsRequired();
                artistPointAward.HasOne<Artist>()
                        .WithMany()
                        .HasForeignKey(pa => pa.ArtistId)
                        .OnDelete(DeleteBehavior.Cascade);

                var missionProgress = builder.Entity<MissionProgress>();
                missionProgress.HasKey(mp => mp.Id);
                missionProgress.Property(mp => mp.Id)
                        .HasConversion(id => id.Value, guid => new MissionProgressId { Value = guid });
                missionProgress.Property(mp => mp.Date)
                        .IsRequired();
                missionProgress.Property(mp => mp.MissionType)
                        .HasConversion(
                                type => type.ToString(),
                                stringValue => Enum.Parse<MissionType>(stringValue))
                        .IsRequired();
                missionProgress.Property(mp => mp.Count)
                        .IsRequired();
                missionProgress.HasOne<IdentityUser<Guid>>()
                        .WithOne()
                        .HasForeignKey<MissionProgress>(mp => mp.UserId)
                        .OnDelete(DeleteBehavior.Cascade);

                var artPieceServed = builder.Entity<ArtPieceServed>();
                artPieceServed.HasKey(aps => aps.Id);
                artPieceServed.Property(aps => aps.Id)
                        .HasConversion(id => id.Value, guid => new ArtPieceServedId { Value = guid });
                artPieceServed.Property(aps => aps.Date)
                        .IsRequired();
                artPieceServed.HasOne<ArtPiece>()
                        .WithMany()
                        .HasForeignKey(aps => aps.ArtPieceId)
                        .OnDelete(DeleteBehavior.Cascade);
        }
}
