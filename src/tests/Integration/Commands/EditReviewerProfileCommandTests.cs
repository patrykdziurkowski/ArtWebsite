using FluentAssertions;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using tests.Integration.Fixtures;
using web;
using web.Features.Reviewers;
using web.Features.Reviewers.EditReviewerProfile;

namespace tests.Integration.Commands;

public class EditReviewerProfileCommandTests : DatabaseTest
{
        private readonly EditReviewerProfileCommand _command;

        public EditReviewerProfileCommandTests(DatabaseTestContext databaseContext) : base(databaseContext)
        {
                _command = Scope.ServiceProvider.GetRequiredService<EditReviewerProfileCommand>();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldThrow_WhenNotOwnerOrAdmin()
        {
                ReviewerId reviewerId1 = await CreateReviewer();
                Guid currentUserId = await DbContext.Users.Select(u => u.Id).SingleAsync();
                ReviewerId reviewerId2 = await CreateReviewer(userName: "otherUser", reviewerName: "someoneElse");

                Func<Task> executingCommand = async () =>
                        await _command.ExecuteAsync(currentUserId, reviewerId2, "newName");

                await executingCommand.Should().ThrowAsync<InvalidOperationException>();
                (await DbContext.Reviewers.SingleAsync(r => r.Id == reviewerId2)).Name.Should().Be("someoneElse");
        }

        [Fact]
        public async Task ExecuteAsync_ReturnsFail_WhenNameTaken()
        {
                ReviewerId reviewerId1 = await CreateReviewer("johnSmith");
                Guid currentUserId = await DbContext.Users.Select(u => u.Id).SingleAsync();
                await CreateReviewer(reviewerName: "takenName");

                Result result = await _command.ExecuteAsync(currentUserId, reviewerId1, "takenName");

                result.IsFailed.Should().BeTrue();
                (await DbContext.Reviewers.SingleAsync(r => r.Id == reviewerId1)).Name.Should().NotBe("takenName");
        }

        [Fact]
        public async Task ExecuteAsync_EditsProfile_WhenOwner()
        {
                ReviewerId reviewerId1 = await CreateReviewer("johnSmith");
                Guid currentUserId = await DbContext.Users.Select(u => u.Id).SingleAsync();

                Result result = await _command.ExecuteAsync(currentUserId, reviewerId1, "newName");

                result.IsSuccess.Should().BeTrue();
                DbContext.Reviewers.Single().Name.Should().Be("newName");
                DbContext.Users.Single().UserName.Should().Be("newName");
        }

        [Fact]
        public async Task ExecuteAsync_EditsProfile_WhenAdmin()
        {
                ReviewerId reviewerId1 = await CreateReviewer("johnSmith");
                IdentityUser<Guid> admin = await DbContext.Users.SingleAsync();
                await UserManager.AddToRoleAsync(admin, Constants.ADMIN_ROLE);

                Guid currentUserId = await DbContext.Users.Select(u => u.Id).SingleAsync();
                ReviewerId reviewerId2 = await CreateReviewer("someoneElse");

                Result result = await _command.ExecuteAsync(currentUserId, reviewerId2, "newName");

                result.IsSuccess.Should().BeTrue();
                (await DbContext.Reviewers.SingleAsync(r => r.Id == reviewerId2)).Name.Should().Be("newName");
        }
}
