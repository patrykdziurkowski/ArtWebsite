using FluentAssertions;
using FluentResults;
using Microsoft.Extensions.DependencyInjection;
using tests.Integration.Fixtures;
using web.Features.ArtPieces;
using web.Features.Likes;
using web.Features.Likes.LikeArtPiece;
using web.Features.Reviewers;

namespace tests.Integration.Commands;

public class LikeArtPieceCommandTests : DatabaseBase
{
        private readonly LikeArtPieceCommand _command;

        public LikeArtPieceCommandTests(DatabaseTestContext databaseContext)
                : base(databaseContext)
        {
                _command = Scope.ServiceProvider.GetRequiredService<LikeArtPieceCommand>();
        }

        [Fact]
        public async Task Execute_ShouldReturnFail_WhenLikedTooManyArtPieces()
        {
                List<ArtPieceId> artPieceIds = await CreateArtistUserWithArtPieces();
                Guid currentUserId = DbContext.Users.First().Id;
                List<Result<Like>> results = [];

                for (int i = 0; i < 6; ++i)
                {
                        Result<Like> result = await _command.ExecuteAsync(currentUserId, artPieceIds[i]);
                        results.Add(result);
                }

                DbContext.Likes.Should().HaveCount(5);
                for (int i = 0; i < 5; ++i)
                {
                        results[i].IsSuccess.Should().BeTrue();
                }
                results[5].IsFailed.Should().BeTrue();
        }

        [Fact]
        public async Task Execute_ShouldLikeArtPiece_WhenSuccess()
        {
                List<ArtPieceId> artPieceIds = await CreateArtistUserWithArtPieces();
                Guid currentUserId = DbContext.Users.First().Id;

                await _command.ExecuteAsync(currentUserId, artPieceIds.First());

                DbContext.Likes.Should().HaveCount(1);
        }
}
