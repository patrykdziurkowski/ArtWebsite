using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using tests.Integration.Fixtures;
using web.Features.ArtPieces;
using web.Features.Likes;
using web.Features.Likes.LoadLikes;

namespace tests.Integration.Queries;

public class LikesQueryTests : DatabaseBase
{
        private readonly LikesQuery _command;

        public LikesQueryTests(DatabaseTestContext databaseContext)
                : base(databaseContext)
        {
                _command = Scope.ServiceProvider.GetRequiredService<LikesQuery>();
        }

        [Fact]
        public async Task Execute_ShouldReturnEmpty_WhenNoLikesForGivenReviewer()
        {
                await CreateArtistUserWithArtPieces();
                Guid currentUserId = DbContext.Users.First().Id;
                List<Like> likes = _command.Execute(currentUserId, 10);

                likes.Should().BeEmpty();
        }

        [Fact]
        public async Task Execute_ShouldReturnLikes_WhenTheyExist()
        {
                List<ArtPieceId> artPieceIds = await CreateArtistUserWithArtPieces();
                Guid currentUserId = await CreateReviewerWith20Likes(artPieceIds);

                List<Like> likes = _command.Execute(currentUserId, 10);

                likes.Should().HaveCount(10);
        }

        [Fact]
        public async Task Execute_ShouldReturnSomeLikes_WhenOffset()
        {
                List<ArtPieceId> artPieceIds = await CreateArtistUserWithArtPieces();
                Guid currentUserId = await CreateReviewerWith20Likes(artPieceIds);

                List<Like> likes = _command.Execute(currentUserId, 10, 17);

                likes.Should().HaveCount(3);
        }

}
