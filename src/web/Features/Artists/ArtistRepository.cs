using Microsoft.EntityFrameworkCore;
using web.Data;

namespace web.Features.Artists;

public class ArtistRepository(ApplicationDbContext dbContext)
{
        public async Task<Artist?> GetByIdAsync(ArtistId artistId)
        {
                Artist? artist = await dbContext.Artists
                        .SingleOrDefaultAsync(a => a.Id == artistId);

                return await InitializeArtistAsync(artist);
        }

        public async Task<Artist?> GetByNameAsync(string name)
        {
                Artist? artist = await dbContext.Artists
                        .SingleOrDefaultAsync(a => a.Name == name);

                return await InitializeArtistAsync(artist);
        }

        public async Task<Artist?> GetByUserIdAsync(Guid userId)
        {
                Artist? artist = await dbContext.Artists
                        .SingleOrDefaultAsync(a => a.UserId == userId);

                return await InitializeArtistAsync(artist);
        }

        public async Task SaveChangesAsync(Artist artist)
        {
                if (dbContext.Entry(artist).State == EntityState.Detached)
                {
                        dbContext.Artists.Add(artist);
                }

                if (artist.DomainEvents.Contains(new ArtistDeactivatedEvent()))
                {
                        dbContext.ArtPieceTags.RemoveRange(
                                dbContext.ArtPieceTags.Where(apt => dbContext.ArtPieces
                                        .Where(ap => ap.ArtistId == artist.Id)
                                        .Select(ap => ap.Id)
                                        .Contains(apt.ArtPieceId)));

                        dbContext.Boosts.RemoveRange(dbContext.Boosts.Where(b => b.ArtistId == artist.Id));
                        dbContext.ArtPieces.RemoveRange(dbContext.ArtPieces.Where(ap => ap.ArtistId == artist.Id));
                        dbContext.Artists.Remove(artist);
                }

                artist.ClearDomainEvents();
                await dbContext.SaveChangesAsync();
        }

        private async Task<Artist?> InitializeArtistAsync(Artist? artist)
        {
                if (artist is null)
                {
                        return null;
                }

                // a somewhat dirty workaround for initializing artist with an active boost
                // without arbitrarily adding navigation properties to Artist domain class
                // or exposing the property's setter which we wanna keep encapsulated
                Boost? activeBoost = await dbContext.Boosts.SingleOrDefaultAsync(b =>
                        b.ArtistId == artist.Id
                        && b.ExpirationDate >= DateTimeOffset.UtcNow);
                typeof(Artist).GetProperty(nameof(Artist.ActiveBoost))!.SetValue(artist, activeBoost);

                return artist;
        }
}
