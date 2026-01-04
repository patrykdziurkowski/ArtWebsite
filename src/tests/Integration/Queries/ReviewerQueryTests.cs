using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using tests.Integration.Fixtures;
using web.Features.Reviewers;
using web.Features.Reviewers.Index;

namespace tests.Integration.Queries;

public class ReviewerQueryTests : DatabaseTest
{
        private readonly ReviewerQuery _query;

        public ReviewerQueryTests(DatabaseTestContext databaseContext) : base(databaseContext)
        {
                _query = Scope.ServiceProvider.GetRequiredService<ReviewerQuery>();
        }

        [Fact]
        public async Task ExecuteAsync_ReturnsReviewer_IfExists()
        {
                IdentityUser<Guid> user = new("JohnSmith");
                await UserManager.CreateAsync(user);
                ReviewerId reviewerId = new();
                DbContext.Reviewers.Add(new Reviewer()
                {
                        Id = reviewerId,
                        Name = "SomeUser123",
                        UserId = user.Id,
                });
                await DbContext.SaveChangesAsync();


                ReviewerProfileDto? obtainedReviewer = await _query.ExecuteAsync(user.Id, "SomeUser123");

                obtainedReviewer.Should().NotBeNull();
                obtainedReviewer.Id.Should().Be(reviewerId);
                obtainedReviewer.CurrentUserOwnsThisProfile.Should().BeTrue();
                obtainedReviewer.IsCurrentUserAdmin.Should().BeFalse();
        }
}
