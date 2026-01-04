using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using tests.Integration.Fixtures;
using web;
using web.Features.Reviews;
using web.Features.Reviews.EditReview;

namespace tests.Integration.Commands;

public class EditReviewCommandTests : DatabaseTest
{
        private readonly EditReviewCommand _command;

        public EditReviewCommandTests(DatabaseTestContext databaseContext) : base(databaseContext)
        {
                _command = Scope.ServiceProvider.GetRequiredService<EditReviewCommand>();
        }

        [Fact]
        public async Task ExecuteAsync_Throws_WhenCurrentUserNotAdminOrOwner()
        {
                await CreateReviewer("currentUser", "currentUser");
                Guid currentUserId = await DbContext.Users.Select(u => u.Id).SingleAsync();
                var artPiecesToReview = await CreateArtistUserWithArtPieces();
                await CreateReviewerWithReviewsForArtPieces(artPiecesToReview);
                ReviewId someoneElsesReviewId = await DbContext.Reviews.Select(r => r.Id).FirstAsync();

                Func<Task> executingCommand = async () =>
                        await _command.ExecuteAsync(currentUserId, someoneElsesReviewId, new Rating(4), "New comment");

                await executingCommand.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task ExecuteAsync_EditsReview_WhenCurrentUserIsOwner()
        {
                var artPiecesToReview = await CreateArtistUserWithArtPieces();
                Guid artistUserId = await DbContext.Users.Select(u => u.Id).SingleAsync();
                await CreateReviewerWithReviewsForArtPieces(artPiecesToReview);
                Guid currentUserId = await DbContext.Users
                        .Select(u => u.Id)
                        .SingleAsync(id => id != artistUserId);
                ReviewId reviewId = await DbContext.Reviews.Select(r => r.Id).FirstAsync();

                await _command.ExecuteAsync(currentUserId, reviewId, new Rating(4), "New comment");

                Review review = await DbContext.Reviews.SingleAsync(r => r.Id == reviewId);
                review.Comment.Should().Be("New comment");
                review.Rating.Should().Be(new Rating(4));
        }

        [Fact]
        public async Task ExecuteAsync_EditsReview_WhenCurrentUserIsAdmin()
        {
                var artPiecesToReview = await CreateArtistUserWithArtPieces();
                IdentityUser<Guid> artistUser = await DbContext.Users.SingleAsync();
                await CreateReviewerWithReviewsForArtPieces(artPiecesToReview);
                ReviewId reviewId = await DbContext.Reviews.Select(r => r.Id).FirstAsync();
                await UserManager.AddToRoleAsync(artistUser, Constants.ADMIN_ROLE);

                await _command.ExecuteAsync(artistUser.Id, reviewId, new Rating(4), "New comment");

                Review review = await DbContext.Reviews.SingleAsync(r => r.Id == reviewId);
                review.Comment.Should().Be("New comment");
                review.Rating.Should().Be(new Rating(4));
        }
}
