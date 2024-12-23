using FluentResults;
using Microsoft.EntityFrameworkCore;
using web.data;

namespace web.features.artist.SetupArtist;

public class SetupArtistCommand
{
        private readonly ApplicationDbContext _dbContext;

        public SetupArtistCommand(ApplicationDbContext dbContext)
        {
                _dbContext = dbContext;
        }

        public async Task<Result<Artist>> Execute(string name, string summary)
        {
                if (await _dbContext.Artists.AnyAsync(a => a.Name == name))
                {
                        return Result.Fail($"An artist with name '{name}' already exists.");
                }

                _dbContext.Add(new Artist(name, summary));
                await _dbContext.SaveChangesAsync();
                return Result.Ok();
        }
}
