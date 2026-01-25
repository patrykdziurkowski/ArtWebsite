using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Features.Artists;
using web.Features.Reviewers;
using web.Features.Tags;

namespace web.Features.Search;

// this warning is disabled since the suggestion doesn't get evaluated to SQL by EFCore
#pragma warning disable CA1862 // Use the 'StringComparison' method overloads to perform case-insensitive string comparisons

public class SearchByNameQuery(ApplicationDbContext dbContext)
{
        private const int ELEMENTS_TO_LOAD_PER_CATEGORY = 5;

        public async Task<SearchDto> ExecuteAsync(string name)
        {
                var nameLowerCase = name.ToLower();

                List<Tag> tagsTask = await dbContext.Tags
                        .Where(t => t.Name.ToLower().Contains(nameLowerCase))
                        .OrderBy(t => EF.Functions.Like(t.Name.ToLower(), $"{nameLowerCase}%") ? 0 : 1)
                        .ThenBy(t => t.Name.Length)
                        .ThenBy(t => t.Name)
                        .Take(ELEMENTS_TO_LOAD_PER_CATEGORY)
                        .ToListAsync();

                List<Artist> artistsTask = await dbContext.Artists
                        .Where(t => t.Name.ToLower().Contains(nameLowerCase))
                        .OrderBy(t => EF.Functions.Like(t.Name.ToLower(), $"{nameLowerCase}%") ? 0 : 1)
                        .ThenBy(t => t.Name.Length)
                        .ThenBy(t => t.Name)
                        .Take(ELEMENTS_TO_LOAD_PER_CATEGORY)
                        .ToListAsync();

                List<Reviewer> reviewersTask = await dbContext.Reviewers
                        .Where(t => t.Name.ToLower().Contains(nameLowerCase))
                        .OrderBy(t => EF.Functions.Like(t.Name.ToLower(), $"{nameLowerCase}%") ? 0 : 1)
                        .ThenBy(t => t.Name.Length)
                        .ThenBy(t => t.Name)
                        .Take(ELEMENTS_TO_LOAD_PER_CATEGORY)
                        .ToListAsync();

                return new SearchDto
                {
                        Tags = tagsTask,
                        Artists = artistsTask,
                        Reviewers = reviewersTask,
                };
        }

}
