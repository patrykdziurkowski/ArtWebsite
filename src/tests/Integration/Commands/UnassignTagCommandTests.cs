using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using tests.Integration.Fixtures;
using web;
using web.Features.ArtPieces;
using web.Features.Tags;
using web.Features.Tags.UnassignTag;

namespace tests.Integration.Commands;

public class UnassignTagCommandTests : DatabaseTest
{
        private readonly UnassignTagCommand _command;

        public UnassignTagCommandTests(DatabaseTestContext databaseContext) : base(databaseContext)
        {
                _command = Scope.ServiceProvider.GetRequiredService<UnassignTagCommand>();
        }

        [Fact]
        public async Task ExecuteAsync_Throws_IfNotAdminOrOwner()
        {
                List<ArtPieceId> artPieceIds = await CreateArtistUserWithArtPieces();
                ArtPieceId artPieceId = artPieceIds.First();

                Tag tagToUnassign = new()
                {
                        Name = "uniqueTag",
                };
                DbContext.Tags.Add(tagToUnassign);
                DbContext.ArtPieceTags.Add(new ArtPieceTag
                {
                        ArtPieceId = artPieceId,
                        TagId = tagToUnassign.Id,
                });
                DbContext.ArtPieceTags.Add(new ArtPieceTag
                {
                        ArtPieceId = artPieceIds.Last(),
                        TagId = tagToUnassign.Id,
                });
                await DbContext.SaveChangesAsync();

                string tagNameToUnassign = DbContext.Tags.Single(t => t.Id == tagToUnassign.Id).Name;

                Func<Task> executing = async () => await _command.ExecuteAsync(Guid.NewGuid(), artPieceId, tagNameToUnassign);

                await executing.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task ExecuteAsync_RemovesTagFromArtPieceTags_WhenMoreOfItsOccurrencesExist_AsArtistOwner()
        {
                List<ArtPieceId> artPieceIds = await CreateArtistUserWithArtPieces();
                Guid currentUserId = DbContext.Users.Single().Id;
                ArtPieceId artPieceId = artPieceIds.First();

                Tag tagToUnassign = new()
                {
                        Name = "someTag",
                };
                DbContext.Tags.Add(tagToUnassign);
                DbContext.ArtPieceTags.Add(new ArtPieceTag
                {
                        ArtPieceId = artPieceId,
                        TagId = tagToUnassign.Id,
                });
                DbContext.ArtPieceTags.Add(new ArtPieceTag
                {
                        ArtPieceId = artPieceIds.Last(),
                        TagId = tagToUnassign.Id,
                });
                await DbContext.SaveChangesAsync();

                int numberOfArtPiecesWithThisTag = DbContext.ArtPieceTags.Count(apt => apt.TagId == tagToUnassign.Id);
                string tagNameToUnassign = DbContext.Tags.Single(t => t.Id == tagToUnassign.Id).Name;

                await _command.ExecuteAsync(currentUserId, artPieceId, tagNameToUnassign);

                numberOfArtPiecesWithThisTag.Should().BeGreaterThan(1);
                DbContext.ArtPieceTags.Should().NotContain(apt => apt.ArtPieceId == artPieceId && apt.TagId == tagToUnassign.Id);
                DbContext.Tags.Should().Contain(t => t.Name == tagNameToUnassign);
        }

        [Fact]
        public async Task ExecuteAsync_RemovesTagFromArtPieceTags_WhenMoreOfItsOccurrencesExist_AsAdmin()
        {
                List<ArtPieceId> artPieceIds = await CreateArtistUserWithArtPieces();
                Guid currentUserId = DbContext.Users.Single().Id;
                await CreateReviewer();
                IdentityUser<Guid> admin = DbContext.Users.Single(u => u.Id != currentUserId);
                await UserManager.AddToRoleAsync(admin, Constants.ADMIN_ROLE);
                Guid adminId = admin.Id;
                ArtPieceId artPieceId = artPieceIds.First();

                Tag tagToUnassign = new()
                {
                        Name = "someTag",
                };
                DbContext.Tags.Add(tagToUnassign);
                DbContext.ArtPieceTags.Add(new ArtPieceTag
                {
                        ArtPieceId = artPieceId,
                        TagId = tagToUnassign.Id,
                });
                DbContext.ArtPieceTags.Add(new ArtPieceTag
                {
                        ArtPieceId = artPieceIds.Last(),
                        TagId = tagToUnassign.Id,
                });
                await DbContext.SaveChangesAsync();

                int numberOfArtPiecesWithThisTag = DbContext.ArtPieceTags.Count(apt => apt.TagId == tagToUnassign.Id);
                string tagNameToUnassign = DbContext.Tags.Single(t => t.Id == tagToUnassign.Id).Name;

                await _command.ExecuteAsync(adminId, artPieceId, tagNameToUnassign);

                numberOfArtPiecesWithThisTag.Should().BeGreaterThan(1);
                DbContext.ArtPieceTags.Should().NotContain(apt => apt.ArtPieceId == artPieceId && apt.TagId == tagToUnassign.Id);
                DbContext.Tags.Should().Contain(t => t.Name == tagNameToUnassign);
        }


        [Fact]
        public async Task ExecuteAsync_RemovesTagFromArtPieceTagsAndTags_WhenNoOccurrencesLeft()
        {
                List<ArtPieceId> artPieceIds = await CreateArtistUserWithArtPieces();
                Guid currentUserId = DbContext.Users.Single().Id;
                ArtPieceId artPieceId = artPieceIds.First();

                Tag tagToUnassign = new()
                {
                        Name = "uniqueTag",
                };
                DbContext.Tags.Add(tagToUnassign);
                DbContext.ArtPieceTags.Add(new ArtPieceTag
                {
                        ArtPieceId = artPieceId,
                        TagId = tagToUnassign.Id,
                });
                await DbContext.SaveChangesAsync();

                int numberOfArtPiecesWithThisTag = DbContext.ArtPieceTags.Count(apt => apt.TagId == tagToUnassign.Id);

                await _command.ExecuteAsync(currentUserId, artPieceId, tagToUnassign.Name);

                numberOfArtPiecesWithThisTag.Should().Be(1);
                DbContext.ArtPieceTags.Should().NotContain(apt => apt.ArtPieceId == artPieceId && apt.TagId == tagToUnassign.Id);
                DbContext.Tags.Should().NotContain(t => t.Name == tagToUnassign.Name);
        }
}
