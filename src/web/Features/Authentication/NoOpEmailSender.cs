using Microsoft.AspNetCore.Identity;

namespace web.Features.Authentication;

public class NoOpEmailSender(ILogger<NoOpEmailSender> logger) : IEmailSender<IdentityUser<Guid>>
{
        private readonly ILogger<NoOpEmailSender> _logger = logger;

        public Task SendConfirmationLinkAsync(IdentityUser<Guid> user, string email, string confirmationLink)
        {
                _logger.LogInformation("[DEV] Confirmation link for {Email}: {Link}", email, confirmationLink);
                return Task.CompletedTask;
        }

        public Task SendPasswordResetLinkAsync(IdentityUser<Guid> user, string email, string resetLink)
        {
                _logger.LogInformation("[DEV] Password reset link for {Email}: {Link}", email, resetLink);
                return Task.CompletedTask;
        }

        public Task SendPasswordResetCodeAsync(IdentityUser<Guid> user, string email, string resetCode)
        {
                _logger.LogInformation("[DEV] Password reset code for {Email}: {Code}", email, resetCode);
                return Task.CompletedTask;
        }
}