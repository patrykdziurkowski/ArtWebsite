using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using web;
using web.Data;
using web.Features.Artists;
using web.Features.Artists.BoostArtPiece;
using web.Features.Artists.DeactivateArtist;
using web.Features.Artists.SetupArtist;
using web.Features.Artists.UpdateArtistProfile;
using web.Features.ArtPieces;
using web.Features.ArtPieces.DeleteArtPiece;
using web.Features.ArtPieces.EditArtPiece;
using web.Features.ArtPieces.LoadArtPieces;
using web.Features.ArtPieces.UploadArtPiece;
using web.Features.Browse;
using web.Features.Browse.ByTag;
using web.Features.Browse.Index;
using web.Features.Browse.SkipArtPiece;
using web.Features.Images;
using web.Features.Leaderboard.Artist;
using web.Features.Leaderboard.Reviewer;
using web.Features.Missions;
using web.Features.Reviewers;
using web.Features.Reviewers.EditReviewerProfile;
using web.Features.Reviewers.Index;
using web.Features.Reviewers.LikeArtPiece;
using web.Features.Reviewers.LoadLikes;
using web.Features.Reviewers.UnlikeArtPiece;
using web.Features.Reviews.DeleteReview;
using web.Features.Reviews.EditReview;
using web.Features.Reviews.LoadReviews;
using web.Features.Reviews.ReviewArtPiece;
using web.Features.Search;
using web.Features.Shared;
using web.Features.Suspensions;
using web.Features.Tags;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
IServiceCollection services = builder.Services;

string connectionString = builder.Configuration["CONNECTION_STRING"]
        ?? throw new InvalidOperationException("Connection string not found.");
if (builder.Configuration["REVIEW_COOLDOWN_SECONDS"] is null
        || !int.TryParse(builder.Configuration["REVIEW_COOLDOWN_SECONDS"], out _))
{
        throw new InvalidOperationException("Review cooldown duration (in seconds) is not configured or not configured properly.");
}
string rootUserName = builder.Configuration["ROOT_USERNAME"]
        ?? throw new InvalidOperationException("Root user does not have a username set.");
string rootEmail = builder.Configuration["ROOT_EMAIL"]
        ?? throw new InvalidOperationException("Root user does not have an email set.");
string rootPassword = builder.Configuration["ROOT_PASSWORD"]
        ?? throw new InvalidOperationException("Root user does not have a password set.");

services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
services.AddIdentity<IdentityUser<Guid>, IdentityRole<Guid>>(options =>
                options.SignIn.RequireConfirmedAccount = true)
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();
// Make Razor look for Identity Pages in the right place
services.AddRazorPages(o => o.RootDirectory = "/Features/Authentication");
services.ConfigureApplicationCookie(o => o.LoginPath = "/Login");
// Globally require a valid ModelState with a default error view
services.AddControllersWithViews(o => o.Filters.Add<ValidateModelFilter>());
services.Configure<RazorViewEngineOptions>(o =>
{
        // {1} - controller name
        // {0} - action name
        o.ViewLocationFormats.Clear();
        o.ViewLocationFormats.Add("/Features/{1}/{0}.cshtml");
        o.ViewLocationFormats.Add("/Features/{1}s/{0}.cshtml");
        o.ViewLocationFormats.Add("/Features/{1}/{0}{1}/{0}.cshtml");
        o.ViewLocationFormats.Add("/Features/{1}s/{0}{1}/{0}.cshtml");
        o.ViewLocationFormats.Add("/Features/{1}s/{0}{1}s/{0}.cshtml");
        o.ViewLocationFormats.Add("/Features/{1}/{0}/{0}.cshtml");
        o.ViewLocationFormats.Add("/Features/{1}s/{0}/{0}.cshtml");
        o.ViewLocationFormats.Add("/Features/Shared/{0}.cshtml");

        // {1} - page name
        // {0} - view name
        o.PageViewLocationFormats.Clear();
        o.PageViewLocationFormats.Add("/Features/Authentication/{1}.cshtml");
        o.PageViewLocationFormats.Add("/Features/Shared/{1}.cshtml");
        o.PageViewLocationFormats.Add("/Features/Authentication/{0}.cshtml");
        o.PageViewLocationFormats.Add("/Features/Shared/{0}.cshtml");
});


services.AddSignalR();

services.AddAutoMapper(typeof(AutoMapperProfile));

services.AddTransient<SetupArtistCommand>();
services.AddTransient<DeactivateArtistCommand>();
services.AddTransient<DeleteArtPieceCommand>();
services.AddTransient<UploadArtPieceCommand>();
services.AddTransient<UpdateArtistCommand>();
services.AddTransient<RegisterArtPieceServedCommand>();
services.AddTransient<EditReviewerProfileCommand>();
services.AddTransient<DeleteReviewCommand>();
services.AddTransient<BoostArtPieceCommand>();
services.AddTransient<ArtPieceQuery>();
services.AddTransient<SuspendUserCommand>();
services.AddTransient<ArtPieceByTagQuery>();
services.AddTransient<ArtPiecesQuery>();
services.AddTransient<ReviewerReviewsQuery>();
services.AddTransient<ArtistLeaderboardQuery>();
services.AddTransient<EditReviewCommand>();
services.AddTransient<EditArtPieceCommand>();
services.AddTransient<ReviewerLeaderboardQuery>();
services.AddTransient<ArtPieceDetailsQuery>();
services.AddTransient<TodaysMissionQuery>();
services.AddTransient<ImageManager>();
services.AddTransient<ArtPieceReviewsQuery>();
services.AddTransient<UserReviewerQuery>();
services.AddTransient<ReviewerQuery>();
services.AddTransient<SkipArtPieceCommand>();
services.AddTransient<SearchByNameQuery>();
services.AddTransient<ArtPieceTagsQuery>();
services.AddTransient<ReviewArtPieceCommand>();
services.AddTransient<LikeArtPieceCommand>();
services.AddTransient<UnlikeArtPieceCommand>();
services.AddTransient<LikesQuery>();

services.AddTransient<ReviewerRepository>();
services.AddTransient<ArtistRepository>();
services.AddTransient<ArtPieceRepository>();

services.AddSingleton<ImageTaggingQueue>();
services.AddTransient<ImageTagger>();
services.AddHostedService<ImageProcessor>();

services.AddTransient<IMissionGenerator, MissionGenerator>();
services.AddTransient<MissionManager>();

services.AddTransient<IEmailSender, NoOpEmailSender>(); // This doesn't actually send an email.

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
        app.UseDeveloperExceptionPage();
        using IServiceScope scope = app.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();
}
else
{
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this
        // for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
}

using (IServiceScope scope = app.Services.CreateScope())
{
        await CreateRolesIfNotPresentAsync(scope);
        await CreateRootUserIfNotPresentAsync(rootUserName, rootPassword, rootEmail, scope);
}

app.UseStaticFiles();
Directory.CreateDirectory(Path.Combine(builder.Environment.ContentRootPath, "user-images"));
app.UseStaticFiles(new StaticFileOptions
{
        FileProvider = new PhysicalFileProvider(
                Path.Combine(builder.Environment.ContentRootPath, "user-images")),
        RequestPath = "/user-images"
});

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseMiddleware<SuspensionMiddleware>();
app.UseAuthorization();
app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();
app.MapRazorPages();

app.MapHub<TagsHub>("/tagshub");

await app.RunAsync();

static async Task CreateRolesIfNotPresentAsync(IServiceScope scope)
{
        RoleManager<IdentityRole<Guid>> roleManager = scope.ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        string[] roles = [Constants.ADMIN_ROLE, Constants.ARTIST_ROLE];
        foreach (string role in roles)
        {
                if (await roleManager.RoleExistsAsync(role) == false)
                {
                        await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                }
        }
}

static async Task CreateRootUserIfNotPresentAsync(string rootUserName, string rootPassword, string rootEmail, IServiceScope scope)
{
        UserManager<IdentityUser<Guid>> userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser<Guid>>>();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        if (await userManager.FindByNameAsync(rootUserName) is null)
        {
                IdentityUser<Guid> root = new(rootUserName)
                {
                        Email = rootEmail,
                        EmailConfirmed = true,
                };

                IdentityResult result = await userManager.CreateAsync(root, rootPassword);
                if (!result.Succeeded)
                {
                        throw new InvalidOperationException("Unable to create the root user: " + string.Join(',', result.Errors.Select(e => e.Description)));
                }

                dbContext.Reviewers.Add(new Reviewer()
                {
                        Name = rootUserName,
                        UserId = root.Id,
                });
                await dbContext.SaveChangesAsync();

                IdentityResult roleResult = await userManager.AddToRoleAsync(root, Constants.ADMIN_ROLE);
                if (!roleResult.Succeeded)
                {
                        throw new InvalidOperationException("Unable to add the root user to the administrator role: " + string.Join(',', roleResult.Errors.Select(e => e.Description)));
                }
        }
}

// for access from integration tests
public partial class Program { }
