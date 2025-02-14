using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using web.Data;
using web.Features.Artists;
using web.Features.ArtPieces;
using web.Features.Reviews;

namespace tests.Integration.Fixtures;

[Collection("Database collection")]
public class DatabaseBase : IDisposable
{
        public ApplicationDbContext DbContext { get; init; }
        public IServiceScope Scope { get; init; }
        public UserManager<IdentityUser<Guid>> UserManager { get; init; }

        public DatabaseBase(DatabaseTestContext databaseContext)
        {
                Scope = databaseContext.Services.CreateScope();
                DbContext = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                UserManager = Scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser<Guid>>>();
                DbContext.Database.BeginTransaction();
        }

        public void Dispose()
        {
                RemoveArtPieceImages();
                DbContext.Database.RollbackTransaction();
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
                ArtistId artistId = new();
                DbContext.Artists.Add(
                        new Artist(artistId, user.Id, artistName,
                                "A profile summary for an artist."));
                await DbContext.SaveChangesAsync();
                return artistId;
        }

        public async Task Create6ArtPiecesForArtist(ArtistId artistId)
        {
                for (int i = 0; i < 6; ++i)
                {
                        ArtPiece artPiece = new($"somePath{i}", "description", artistId);
                        await DbContext.ArtPieces.AddAsync(artPiece);
                        await DbContext.SaveChangesAsync();
                }
        }

        public async Task<List<ArtPieceId>> CreateArtistUserWithArtPieces()
        {
                IdentityUser<Guid> user = new("johnSmith");
                await UserManager.CreateAsync(user);
                ArtistId artistId = new();
                DbContext.Artists.Add(
                        new Artist(artistId, user.Id, "ArtistName",
                                "A profile summary for an artist."));
                List<ArtPieceId> artPieceIds = [];
                for (int i = 0; i < 20; ++i)
                {
                        ArtPiece artPiece = new($"somePath", "description", artistId);
                        await DbContext.ArtPieces.AddAsync(artPiece);
                        artPieceIds.Add(artPiece.Id);
                }
                await DbContext.SaveChangesAsync();
                return artPieceIds;
        }

        public async Task<Guid> CreateUserWith20Reviews(List<ArtPieceId> artPiecesToReview)
        {
                IdentityUser<Guid> user = new("johnSmith2");
                await UserManager.CreateAsync(user);
                foreach (ArtPieceId artPieceId in artPiecesToReview)
                {
                        Review review = new()
                        {
                                Comment = "Some comment with a descriptive opinion that's long enough. Some comment with a descriptive opinion that's long enough.",
                                ArtPieceId = artPieceId,
                                ReviewerId = user.Id,
                        };
                        await DbContext.AddAsync(review);
                }
                await DbContext.SaveChangesAsync();
                return user.Id;
        }

        public FormFile GetExampleFile()
        {
                MemoryStream stream = new(Encoding.UTF8.GetBytes("Test file"));
                return new FormFile(stream, 0, stream.Length, "file", "myFile")
                {
                        Headers = new HeaderDictionary(),
                        ContentType = "image/jpeg"
                };
        }

}
