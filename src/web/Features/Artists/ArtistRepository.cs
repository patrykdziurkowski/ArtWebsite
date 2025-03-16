using Microsoft.EntityFrameworkCore;
using web.Data;

namespace web.Features.Artists;

public class ArtistRepository(ApplicationDbContext dbContext)
{
        public async Task<Artist?> GetByIdAsync(ArtistId artistId)
        {
                return await dbContext.Artists.FirstOrDefaultAsync(a => a.Id == artistId);
        }

        public async Task<Artist?> GetByNameAsync(string name)
        {
                return await dbContext.Artists.FirstOrDefaultAsync(a => a.Name == name);
        }

        public async Task SaveChangesAsync()
        {
                await dbContext.SaveChangesAsync();
        }

        public async Task SaveChangesAsync(Artist artist)
        {
                dbContext.Artists.Add(artist);
                await dbContext.SaveChangesAsync();
        }
}
