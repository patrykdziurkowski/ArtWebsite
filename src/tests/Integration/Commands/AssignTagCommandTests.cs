using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using tests.Integration.Fixtures;
using web;
using web.Features.ArtPieces;
using web.Features.Tags;
using web.Features.Tags.AssignTag;

namespace tests.Integration.Commands;

public class AssignTagCommandTests : DatabaseTest
{
        private readonly AssignTagCommand _command;

        public AssignTagCommandTests(DatabaseTestContext databaseContext) : base(databaseContext)
        {
                _command = Scope.ServiceProvider.GetRequiredService<AssignTagCommand>();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldThrow_WhenCurrentUserNotOwnerOrArtist()
        {
                List<ArtPieceId> artPieceIds = await CreateArtistUserWithArtPieces();
                ArtPieceId artPieceId = artPieceIds.First();

                Func<Task> executing = async () => await _command.ExecuteAsync(Guid.NewGuid(), artPieceId, "newTag");

                await executing.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldAddNewTag_IfDidntExist()
        {
                List<ArtPieceId> artPieceIds = await CreateArtistUserWithArtPieces();
                Guid currentUserId = DbContext.Users.Single().Id;
                ArtPieceId artPieceId = artPieceIds.First();

                DbContext.Tags.Should().NotContain(t => t.Name == "newTag");
                
                await _command.ExecuteAsync(currentUserId, artPieceId, "newTag");

                Tag? createdTag = DbContext.Tags.SingleOrDefault(t => t.Name == "newTag");
                createdTag.Should().NotBeNull();
                DbContext.ArtPieceTags.Should().Contain(apt => apt.TagId == createdTag.Id && apt.ArtPieceId == artPieceId);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldLinkToExistingTag_WhenExisted()
        {
                List<ArtPieceId> artPieceIds = await CreateArtistUserWithArtPieces();
                Guid currentUserId = DbContext.Users.Single().Id;
                ArtPieceId artPieceId = artPieceIds.First();

                DbContext.Tags.Add(new Tag
                {
                        Name = "newTag",
                });
                await DbContext.SaveChangesAsync();

                Tag? existingTag = DbContext.Tags.SingleOrDefault(t => t.Name == "newTag");
                existingTag.Should().NotBeNull();
                DbContext.ArtPieceTags.Should().NotContain(apt => apt.TagId == existingTag.Id && apt.ArtPieceId == artPieceId);

                await _command.ExecuteAsync(currentUserId, artPieceId, "newTag");

                existingTag.Should().NotBeNull();
                DbContext.ArtPieceTags.Should().Contain(apt => apt.TagId == existingTag.Id && apt.ArtPieceId == artPieceId);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldWorkForAdmin()
        {
                List<ArtPieceId> artPieceIds = await CreateArtistUserWithArtPieces();
                Guid currentUserId = DbContext.Users.Single().Id;
                await CreateReviewer();
                IdentityUser<Guid> admin = DbContext.Users.Single(u => u.Id != currentUserId);
                await UserManager.AddToRoleAsync(admin, Constants.ADMIN_ROLE);
                Guid adminId = admin.Id;
                ArtPieceId artPieceId = artPieceIds.First();

                await _command.ExecuteAsync(adminId, artPieceId, "newTag");

                Tag? createdTag = DbContext.Tags.SingleOrDefault(t => t.Name == "newTag");
                createdTag.Should().NotBeNull();
                DbContext.ArtPieceTags.Should().Contain(apt => apt.TagId == createdTag.Id && apt.ArtPieceId == artPieceId);
        }
}
