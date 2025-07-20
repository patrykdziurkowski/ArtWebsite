using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using web.Data;
using web.Features.ArtPieces;

namespace web.Features.Artists;

public class ArtistRepository(ApplicationDbContext dbContext)
{
        public async Task<Artist?> GetByIdAsync(ArtistId artistId)
        {
                Artist? artist = await dbContext.Artists
                        .SingleOrDefaultAsync(a => a.Id == artistId);
                if (artist is null)
                {
                        return null;
                }

                return InitializeArtist(artist);

        }

        public async Task<Artist?> GetByNameAsync(string name)
        {
                Artist? artist = await dbContext.Artists.SingleOrDefaultAsync(a => a.Name == name);
                if (artist is null)
                {
                        return null;
                }

                return InitializeArtist(artist);
        }

        public async Task<Artist?> GetByUserIdAsync(Guid userId)
        {
                Artist? artist = await dbContext.Artists.SingleOrDefaultAsync(a => a.UserId == userId);
                if (artist is null)
                {
                        return null;
                }

                return InitializeArtist(artist);
        }

        public async Task SaveChangesAsync(Artist artist)
        {
                if (dbContext.Entry(artist).State == EntityState.Detached)
                {
                        dbContext.Artists.Add(artist);
                }

                if (artist.DomainEvents.Contains(new ArtistDeactivatedEvent()))
                {
                        dbContext.Boosts.RemoveRange(dbContext.Boosts.Where(b => b.ArtistId == artist.Id));
                        dbContext.ArtPieces.RemoveRange(dbContext.ArtPieces.Where(ap => ap.ArtistId == artist.Id));
                        dbContext.Artists.Remove(artist);
                }

                await dbContext.SaveChangesAsync();
        }

        private Artist InitializeArtist(Artist artist)
        {
                // workaround to initialize computed properties or arbitrarily load related data
                Artist newArtist = new()
                {
                        Id = artist.Id,
                        Name = artist.Name,
                        Summary = artist.Summary,
                        UserId = artist.UserId,
                        ActiveBoost = dbContext.Boosts.SingleOrDefault(b => b.ArtistId == artist.Id
                                                && b.ExpirationDate >= DateTimeOffset.UtcNow)
                };
                dbContext.Entry(artist).State = EntityState.Detached;
                dbContext.Artists.Attach(newArtist);
                return newArtist;
        }
}
