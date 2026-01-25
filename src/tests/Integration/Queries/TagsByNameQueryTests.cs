using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using tests.Integration.Fixtures;
using web.Features.Tags;

namespace tests.Integration.Queries;

public class TagsByNameQueryTests : DatabaseTest
{
        private readonly TagsByNameQuery _query;

        public TagsByNameQueryTests(DatabaseTestContext databaseContext) : base(databaseContext)
        {
                _query = Scope.ServiceProvider.GetRequiredService<TagsByNameQuery>();
        }

        [Fact]
        public async Task ExecuteAsync_ReturnsEmpty_WhenNoTagsExist()
        {
                List<Tag> tags = await _query.ExecuteAsync(string.Empty);

                tags.Should().BeEmpty();
        }

        [Fact]
        public async Task ExecuteAsync_ReturnsEmpty_WhenTagsExistButDontMatch()
        {
                await DbContext.Tags.AddRangeAsync(new Tag()
                {
                        Name = "someTag1",
                },
                new Tag()
                {
                        Name = "other tag",
                });
                await DbContext.SaveChangesAsync();

                List<Tag> tags = await _query.ExecuteAsync("someVerySpecificTag123");

                tags.Should().BeEmpty();
                DbContext.Tags.Should().HaveCount(2);
        }

        [Fact]
        public async Task ExecuteAsync_ReturnsMatchingTags_WhenTagsExistAndMatch()
        {
                await DbContext.Tags.AddRangeAsync(new Tag()
                {
                        Name = "someTag1",
                },
                new Tag()
                {
                        Name = "other tag",
                });
                await DbContext.SaveChangesAsync();

                List<Tag> tags = await _query.ExecuteAsync("Tag");

                tags.Should().HaveCount(2);
                DbContext.Tags.Should().HaveCount(2);
        }

        [Fact]
        public async Task ExecuteAsync_Returns5MatchingTags_When6AreMatching()
        {
                await DbContext.Tags.AddRangeAsync(new Tag()
                {
                        Name = "someTag1",
                },
                new Tag()
                {
                        Name = "other Tag",
                },
                new Tag()
                {
                        Name = "another Tag",
                },
                new Tag()
                {
                        Name = "123Tag456",
                },
                new Tag()
                {
                        Name = "Tag different",
                },
                new Tag()
                {
                        Name = "Tag",
                });
                await DbContext.SaveChangesAsync();

                List<Tag> tags = await _query.ExecuteAsync("Tag");

                tags.Should().HaveCount(5);
                DbContext.Tags.Should().HaveCount(6);
        }

        [Fact]
        public async Task ExecuteAsync_IsNotCaseSensitiveAndOrdersTagsCorrectly()
        {
                await DbContext.Tags.AddRangeAsync(new Tag()
                {
                        Name = "someTag1",
                },
                new Tag()
                {
                        Name = "other Tag",
                },
                new Tag()
                {
                        Name = "another Tag",
                },
                new Tag()
                {
                        Name = "123Tag456",
                },
                new Tag()
                {
                        Name = "Tag different",
                },
                new Tag()
                {
                        Name = "Tag",
                },
                new Tag()
                {
                        Name = "nothing",
                });
                await DbContext.SaveChangesAsync();

                List<Tag> tags = await _query.ExecuteAsync("tag");

                tags.Should().HaveCount(5);
                DbContext.Tags.Should().HaveCount(7);
                tags[0].Name.Should().Be("Tag");
                tags[1].Name.Should().Be("Tag different");
                tags.Should().NotContain(t => t.Name == "nothing");
        }

}
