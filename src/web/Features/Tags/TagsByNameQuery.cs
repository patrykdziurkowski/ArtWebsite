using Microsoft.EntityFrameworkCore;
using web.Data;

namespace web.Features.Tags;

public class TagsByNameQuery(ApplicationDbContext dbContext)
{
        private const int TAGS_TO_LOAD = 5;

        public async Task<List<Tag>> ExecuteAsync(string tagName)
        {
                return await dbContext.Tags
                        .Where(t => t.Name.Contains(tagName))
                        .OrderBy(t => t.Name)
                        .Take(TAGS_TO_LOAD)
                        .ToListAsync();
        }
}
