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
using web.Features.ArtPieces;
using web.Features.ArtPieces.LoadArtPieces;
using web.Features.ArtPieces.UploadArtPiece;
using web.Features.Browse.ByTag;
using web.Features.Browse.Index;
using web.Features.Leaderboard.Artist;
using web.Features.Leaderboard.Reviewer;
using web.Features.Missions;
using web.Features.Reviewers;
using web.Features.Reviewers.Index;
using web.Features.Reviewers.LikeArtPiece;
using web.Features.Reviewers.LoadLikes;
using web.Features.Reviewers.UnlikeArtPiece;
using web.Features.Reviews.LoadReviews;
using web.Features.Reviews.ReviewArtPiece;
using web.Features.Shared;
using web.Features.Tags;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
IServiceCollection services = builder.Services;

string connectionString = builder.Configuration["CONNECTION_STRING"]
        ?? throw new InvalidOperationException("Connection string not found.");

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
services.AddTransient<UploadArtPieceCommand>();
services.AddTransient<BoostArtPieceCommand>();
services.AddTransient<ArtPieceQuery>();
services.AddTransient<ArtPieceByTagQuery>();
services.AddTransient<ArtPiecesQuery>();
services.AddTransient<ReviewerReviewsQuery>();
services.AddTransient<ArtistLeaderboardQuery>();
services.AddTransient<ReviewerLeaderboardQuery>();
services.AddTransient<ArtPieceReviewsQuery>();
services.AddTransient<UserReviewerQuery>();
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
        RoleManager<IdentityRole<Guid>> roleManager = scope.ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        string[] roles = ["Admin", "Artist"];
        foreach (string role in roles)
        {
                if (await roleManager.RoleExistsAsync(role) == false)
                {
                        await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                }
        }
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
app.UseAuthorization();
app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();
app.MapRazorPages();

app.MapHub<TagsHub>("/tagshub");

await app.RunAsync();

public partial class Program { }
