using Microsoft.EntityFrameworkCore;
using web.Data;

namespace web.Features.Tags;

public class TagsByNameQuery(ApplicationDbContext dbContext)
{
        private const int TAGS_TO_LOAD = 5;

        public async Task<List<Tag>> ExecuteAsync(string tagName)
        {
                var lowerTagName = tagName.ToLower();

#pragma warning disable CA1862 // Use the 'StringComparison' method overloads to perform case-insensitive string comparisons
                List<Tag> tags = await dbContext.Tags
                        .Where(t => t.Name.ToLower().Contains(lowerTagName))
                        .OrderBy(t => EF.Functions.Like(t.Name.ToLower(), $"{lowerTagName}%") ? 0 : 1)
                        .ThenBy(t => t.Name.Length)
                        .ThenBy(t => t.Name)
                        .Take(TAGS_TO_LOAD)
                        .ToListAsync();
#pragma warning restore CA1862 // Use the 'StringComparison' method overloads to perform case-insensitive string comparisons

                return tags;
        }
}
