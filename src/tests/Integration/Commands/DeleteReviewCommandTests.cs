using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using tests.Integration.Fixtures;
using web;
using web.Features.Reviews;
using web.Features.Reviews.DeleteReview;

namespace tests.Integration.Commands;

public class DeleteReviewCommandTests : DatabaseTest
{
        private readonly DeleteReviewCommand _command;

        public DeleteReviewCommandTests(DatabaseTestContext databaseContext) : base(databaseContext)
        {
                _command = Scope.ServiceProvider.GetRequiredService<DeleteReviewCommand>();
        }

        [Fact]
        public async Task ExecuteAsync_Throws_WhenNotAdminOrReviewOwner()
        {
                await CreateReviewer("Some123Reviewer");
                Guid currentUserId = await DbContext.Users.Select(u => u.Id).SingleAsync();
                var artPieceIds = await CreateArtistUserWithArtPieces();
                await CreateReviewerWithReviewsForArtPieces(artPieceIds);
                ReviewId reviewId = await DbContext.Reviews.Select(r => r.Id).FirstAsync();

                Func<Task> executingCommand = async () => await _command.ExecuteAsync(currentUserId, reviewId);

                await executingCommand.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task ExecuteAsync_DeletesReview_WhenAdmin()
        {
                IdentityUser<Guid> admin = new("adminAdmin");
                await UserManager.CreateAsync(admin);
                await UserManager.AddToRoleAsync(admin, Constants.ADMIN_ROLE);

                var artPieceIds = await CreateArtistUserWithArtPieces();
                await CreateReviewerWithReviewsForArtPieces(artPieceIds);
                ReviewId reviewId = await DbContext.Reviews.Select(r => r.Id).FirstAsync();

                await _command.ExecuteAsync(admin.Id, reviewId);

                (await DbContext.Reviews.AnyAsync(r => r.Id == reviewId)).Should().BeFalse();
        }

        [Fact]
        public async Task ExecuteAsync_DeletesReview_WhenReviewOwner()
        {
                var artPieceIds = await CreateArtistUserWithArtPieces();
                await CreateReviewerWithReviewsForArtPieces(artPieceIds);
                ReviewId reviewId = await DbContext.Reviews.Select(r => r.Id).FirstAsync();
                Guid currentUserId = await DbContext.Reviewers
                        .OrderByDescending(r => r.JoinDate)
                        .Select(u => u.UserId)
                        .FirstAsync();

                await _command.ExecuteAsync(currentUserId, reviewId);

                (await DbContext.Reviews.AnyAsync(r => r.Id == reviewId)).Should().BeFalse();
        }
}
