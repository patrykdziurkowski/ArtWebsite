using Ductus.FluentDocker.Common;
using FluentAssertions;
using FluentResults;
using Microsoft.Extensions.DependencyInjection;
using tests.Integration.Fixtures;
using web.Features.Browse;
using web.Features.Browse.SkipArtPiece;

namespace tests.Integration.Commands;

public class SkipArtPieceCommandTests : DatabaseTest
{
    private readonly SkipArtPieceCommand _command;
    private readonly RegisterArtPieceServedCommand _registerArtPieceServedCommand;

    public SkipArtPieceCommandTests(DatabaseTestContext databaseContext) : base(databaseContext)
    {
        _command = Scope.ServiceProvider.GetRequiredService<SkipArtPieceCommand>();
        _registerArtPieceServedCommand = Scope.ServiceProvider.GetRequiredService<RegisterArtPieceServedCommand>();
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsFail_WhenNotEnoughPoints()
    {
        var artPieceIds = await CreateArtistUserWithArtPieces();
        Guid artistUserId = DbContext.Users.Single().Id;
        await CreateReviewerWithReviewsForArtPieces(artPieceIds);
        Guid currentUserId = DbContext.Users.Single(u => u.Id != artistUserId).Id;
        DbContext.Reviewers.Single(r => r.UserId == currentUserId).Points = 0;
        DbContext.SaveChanges();
        await _registerArtPieceServedCommand.ExecuteAsync(currentUserId, artPieceIds.First());
        int reviewerPointsBefore = DbContext.Reviewers.Single(u => u.UserId == currentUserId).Points;
        int activeReviewerPointsBefore = DbContext.Reviewers.Single(u => u.UserId == currentUserId).Points;
        
        Result result = await _command.ExecuteAsync(currentUserId);
        int reviewerPointsAfter = DbContext.Reviewers.Single(u => u.UserId == currentUserId).Points;
        int activeReviewerPointsAfter = DbContext.Reviewers.Single(u => u.UserId == currentUserId).Points;

        result.IsSuccess.Should().BeFalse();
        DbContext.ArtPiecesServed.Should().HaveCount(1);
        reviewerPointsBefore.Should().Be(0);
        reviewerPointsAfter.Should().Be(0);
    }

    [Fact]
    public async Task ExecuteAsync_SubtractsPointsAndRemovesLastRegisteredArtPieceServed_WhenSuccess()
    {
        var artPieceIds = await CreateArtistUserWithArtPieces();
        Guid artistUserId = DbContext.Users.Single().Id;
        await CreateReviewerWithReviewsForArtPieces(artPieceIds);
        Guid currentUserId = DbContext.Users.Single(u => u.Id != artistUserId).Id;
        DbContext.Reviewers.Single(r => r.UserId == currentUserId).Points = 200;
        DbContext.Reviewers.Single(r => r.UserId == currentUserId).ActivePoints = 200;
        DbContext.SaveChanges();
        await _registerArtPieceServedCommand.ExecuteAsync(currentUserId, artPieceIds.First());
        int reviewerPointsBefore = DbContext.Reviewers.Single(u => u.UserId == currentUserId).Points;
        int activeReviewerPointsBefore = DbContext.Reviewers.Single(u => u.UserId == currentUserId).ActivePoints;
        
        Result result = await _command.ExecuteAsync(currentUserId);
        int reviewerPointsAfter = DbContext.Reviewers.Single(u => u.UserId == currentUserId).Points;
        int activeReviewerPointsAfter = DbContext.Reviewers.Single(u => u.UserId == currentUserId).ActivePoints;

        result.IsFailed.Should().BeFalse();
        DbContext.ArtPiecesServed.Single().WasSkipped.Should().BeTrue();
        reviewerPointsBefore.Should().Be(200);
        reviewerPointsAfter.Should().Be(200);
        activeReviewerPointsBefore.Should().Be(200);
        activeReviewerPointsAfter.Should().Be(195);
    }
}
