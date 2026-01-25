using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using tests.Integration.Fixtures;
using web.Features.Search;
using web.Features.Tags;

namespace tests.Integration.Queries;

public class SearchByNameQueryTests : DatabaseTest
{
        private readonly SearchByNameQuery _query;

        public SearchByNameQueryTests(DatabaseTestContext databaseContext) : base(databaseContext)
        {
                _query = Scope.ServiceProvider.GetRequiredService<SearchByNameQuery>();
        }

        [Fact]
        public async Task ExecuteAsync_ReturnsEmpty_WhenNoTagsExist()
        {
                SearchDto result = await _query.ExecuteAsync(string.Empty);

                result.Tags.Should().BeEmpty();
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

                SearchDto result = await _query.ExecuteAsync("someVerySpecificTag123");

                result.Tags.Should().BeEmpty();
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

                SearchDto result = await _query.ExecuteAsync("Tag");

                result.Tags.Should().HaveCount(2);
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

                SearchDto result = await _query.ExecuteAsync("Tag");

                result.Tags.Should().HaveCount(5);
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

                SearchDto result = await _query.ExecuteAsync("tag");

                result.Tags.Should().HaveCount(5);
                DbContext.Tags.Should().HaveCount(7);
                result.Tags[0].Name.Should().Be("Tag");
                result.Tags[1].Name.Should().Be("Tag different");
                result.Tags.Should().NotContain(t => t.Name == "nothing");
        }

        [Fact]
        public async Task ExecuteAsync_ReturnsManyDifferentResults_WhenTheyExist()
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

                await CreateReviewer(reviewerName: "tagUser");
                await CreateArtistUserWithArtPieces(reviewerName: "tagTag123", artistName: "someTagsName");

                SearchDto result = await _query.ExecuteAsync("Tag");

                result.Tags.Should().HaveCount(5);
                result.Reviewers.Should().HaveCount(2);
                result.Artists.Should().HaveCount(1);
                DbContext.Tags.Should().HaveCount(6);
                DbContext.Reviewers.Should().HaveCount(2);
                DbContext.Artists.Should().HaveCount(1);
        }

}
