using Microsoft.AspNetCore.Identity;
using web.Data;

namespace web.Features.Suspensions;

public class SuspendUserCommand(
        UserManager<IdentityUser<Guid>> userManager,
        ApplicationDbContext dbContext)
{
        public async Task<Suspension> ExecuteAsync(
                Guid currentUserId,
                Guid userToSuspendId,
                TimeSpan suspensionDuration,
                string reason,
                DateTimeOffset? now = null)
        {
                now ??= DateTimeOffset.UtcNow;

                IdentityUser<Guid> currentUser = (await userManager.FindByIdAsync(currentUserId.ToString()))!;
                if (await userManager.IsInRoleAsync(currentUser, Constants.ADMIN_ROLE) == false)
                {
                        throw new InvalidOperationException("Unable to suspend this user. Current user is not admin.");
                }

                IdentityUser<Guid> userToSuspend = (await userManager.FindByIdAsync(userToSuspendId.ToString()))!;
                if (await userManager.IsInRoleAsync(userToSuspend, Constants.ADMIN_ROLE))
                {
                        throw new InvalidOperationException("Unable to suspend this user - they're an admin.");
                }

                Suspension suspension = new()
                {
                        IssuingUserId = currentUserId,
                        UserId = userToSuspendId,
                        Reason = reason,
                        IssuedAt = now.Value,
                        ExpiryDate = now.Value.Add(suspensionDuration),
                };

                await dbContext.AddAsync(suspension);
                await dbContext.SaveChangesAsync();
                return suspension;
        }
}
