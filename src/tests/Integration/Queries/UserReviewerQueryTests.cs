using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using tests.Integration.Fixtures;
using web.Features.Reviewers;
using web.Features.Reviewers.Index;

namespace tests.Integration.Queries;

public class UserReviewerQueryTests : DatabaseBase
{
        private readonly UserReviewerQuery _command;
        public UserReviewerQueryTests(DatabaseTestContext databaseContext)
                : base(databaseContext)
        {
                _command = Scope.ServiceProvider.GetRequiredService<UserReviewerQuery>();
        }

        [Fact]
        public async Task Execute_ShouldReturnReviewerId_WhenGivenUserId()
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


                ReviewerId obtainedReviewerId = _command.Execute(user.Id);

                obtainedReviewerId.Should().Be(reviewerId);
        }

}
