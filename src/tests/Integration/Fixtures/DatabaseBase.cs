using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using web.Data;
using web.Features.Artists;
using web.Features.ArtPieces;
using web.Features.Reviewers;
using web.Features.Reviews;

namespace tests.Integration.Fixtures;

[Collection("Database collection")]
public abstract class DatabaseBase : IDisposable
{
        public ApplicationDbContext DbContext { get; init; }
        public IServiceScope Scope { get; init; }
        public UserManager<IdentityUser<Guid>> UserManager { get; init; }

        public DatabaseBase(DatabaseTestContext databaseContext)
        {
                Scope = databaseContext.Services.CreateScope();
                DbContext = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                UserManager = Scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser<Guid>>>();

                DbContext.ArtPieceTags.ExecuteDelete();
                DbContext.Tags.ExecuteDelete();
                DbContext.Boosts.ExecuteDelete();
                DbContext.Likes.ExecuteDelete();
                DbContext.Reviews.ExecuteDelete();
                DbContext.ArtPieces.ExecuteDelete();
                DbContext.Artists.ExecuteDelete();
                DbContext.Reviewers.ExecuteDelete();
                DbContext.UserRoles.ExecuteDelete();
                DbContext.UserLogins.ExecuteDelete();
                DbContext.UserTokens.ExecuteDelete();
                DbContext.UserClaims.ExecuteDelete();
                DbContext.Users.ExecuteDelete();
        }

        public void Dispose()
        {
                RemoveArtPieceImages();
                Scope.Dispose();
        }

        public void RemoveArtPieceImages()
        {
                if (Directory.Exists("user-images"))
                {
                        Directory.Delete("user-images", recursive: true);
                }
        }

        public async Task<ArtistId> CreateUserWithArtistProfile(
                string username = "johnSmith", string artistName = "ArtistName")
        {
                IdentityUser<Guid> user = new(username);
                await UserManager.CreateAsync(user);
                DbContext.Reviewers.Add(new Reviewer()
                {
                        Name = "SomeUser123",
                        UserId = user.Id,
                });
                ArtistId artistId = new();
                DbContext.Artists.Add(new Artist
                {
                        Id = artistId,
                        UserId = user.Id,
                        Name = artistName,
                        Summary = "A profile summary for an artist.",
                });
                await DbContext.SaveChangesAsync();
                return artistId;
        }

        public async Task Create6ArtPiecesForArtist(ArtistId artistId)
        {
                for (int i = 0; i < 6; ++i)
                {
                        ArtPiece artPiece = new()
                        {
                                ImagePath = $"somePath{i}",
                                Description = "description",
                                ArtistId = artistId,
                        };
                        await DbContext.ArtPieces.AddAsync(artPiece);
                        await DbContext.SaveChangesAsync();
                }
        }

        public async Task<List<ArtPieceId>> CreateArtistUserWithArtPieces()
        {
                IdentityUser<Guid> user = new("johnSmith");
                await UserManager.CreateAsync(user);
                DbContext.Reviewers.Add(new Reviewer()
                {
                        Name = "SomeUser123",
                        UserId = user.Id,
                });
                ArtistId artistId = new();
                DbContext.Artists.Add(new Artist
                {
                        Id = artistId,
                        UserId = user.Id,
                        Name = "ArtistName",
                        Summary = "A profile summary for an artist.",
                });
                List<ArtPieceId> artPieceIds = [];
                for (int i = 0; i < 20; ++i)
                {
                        ArtPiece artPiece = new()
                        {
                                ImagePath = "somePath",
                                Description = "description",
                                ArtistId = artistId,
                        };
                        await DbContext.ArtPieces.AddAsync(artPiece);
                        artPieceIds.Add(artPiece.Id);
                }
                await DbContext.SaveChangesAsync();
                return artPieceIds;
        }

        public async Task<Reviewer> CreateReviewerWith20Reviews(List<ArtPieceId> artPiecesToReview)
        {
                IdentityUser<Guid> user = new("johnSmith2");
                await UserManager.CreateAsync(user);
                ReviewerId reviewerId = new();
                Reviewer reviewer = new()
                {
                        Id = reviewerId,
                        Name = "SomeUser123",
                        UserId = user.Id,
                };
                DbContext.Reviewers.Add(reviewer);
                foreach (ArtPieceId artPieceId in artPiecesToReview)
                {
                        Review review = new()
                        {
                                Comment = "Some comment with a descriptive opinion that's long enough. Some comment with a descriptive opinion that's long enough.",
                                Rating = new Rating(5),
                                ArtPieceId = artPieceId,
                                ReviewerId = reviewerId,
                        };
                        await DbContext.AddAsync(review);
                }
                await DbContext.SaveChangesAsync();
                return reviewer;
        }

        public async Task<Guid> CreateReviewerWithLikes(List<ArtPieceId> artPiecesToLike)
        {
                IdentityUser<Guid> user = new("johnSmith2");
                await UserManager.CreateAsync(user);
                ReviewerId reviewerId = new();
                DbContext.Reviewers.Add(new Reviewer()
                {
                        Id = reviewerId,
                        Name = "SomeUser123",
                        UserId = user.Id,
                });
                foreach (ArtPieceId artPieceId in artPiecesToLike)
                {
                        Like like = new()
                        {
                                ArtPieceId = artPieceId,
                                ReviewerId = reviewerId,
                        };
                        await DbContext.AddAsync(like);
                }
                await DbContext.SaveChangesAsync();
                return user.Id;
        }

        public FormFile GetExampleFile()
        {
                string image = Path.GetFullPath(Path.Combine(
                        GetType().Assembly.Location,
                        "..",
                        "..",
                        "..",
                        "..",
                        "resources",
                        "exampleImage.png"));
                if (!File.Exists(image))
                {
                        throw new InvalidOperationException("Unable to find a sample image for a test.");
                }

                FileStream stream = File.OpenRead(image);
                return new FormFile(stream, 0, stream.Length, "file", Path.GetFileName(image))
                {
                        Headers = new HeaderDictionary(),
                        ContentType = "image/png"
                };
        }

}
