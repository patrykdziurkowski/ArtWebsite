using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using web.Data;
using web.Features.Artists.DeactivateArtist;
using web.Features.Artists.SetupArtist;
using web.Features.ArtPieces.Index;
using web.Features.ArtPieces.LoadArtPieces;
using web.Features.ArtPieces.UploadArtPiece;
using web.Features.Likes.LikeArtPiece;
using web.Features.Likes.LoadLikes;
using web.Features.Reviewers.Index;
using web.Features.Reviews.LoadReviews;
using web.Features.Reviews.ReviewArtPiece;
using web.Features.Shared;

var builder = WebApplication.CreateBuilder(args);
string connectionString = builder.Configuration["CONNECTION_STRING"]
                ?? throw new InvalidOperationException("Connection string not found.");
builder.Services
        .AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
builder.Services
        .AddIdentity<IdentityUser<Guid>, IdentityRole<Guid>>(options =>
                options.SignIn.RequireConfirmedAccount = true)
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();
builder.Services.AddRazorPages(o => // Pages are used for Identity only
{
        o.RootDirectory = "/Features/Authentication"; // Make Razor look for Identity Pages in the right place
});
builder.Services.ConfigureApplicationCookie(o =>
{
        o.LoginPath = "/Login";
});

builder.Services.AddControllersWithViews(o =>
{
        o.Filters.Add<ValidateModelFilter>(); // Globally require a valid ModelState with a default error view
});
builder.Services.Configure<RazorViewEngineOptions>(o =>
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

builder.Services.AddTransient<SetupArtistCommand>();
builder.Services.AddTransient<DeactivateArtistCommand>();
builder.Services.AddTransient<UploadArtPieceCommand>();
builder.Services.AddTransient<ArtPieceQuery>();
builder.Services.AddTransient<ArtPiecesQuery>();
builder.Services.AddTransient<ReviewsQuery>();
builder.Services.AddTransient<UserReviewerQuery>();
builder.Services.AddTransient<ReviewArtPieceCommand>();
builder.Services.AddTransient<LikeArtPieceCommand>();
builder.Services.AddTransient<LikesQuery>();
builder.Services.AddTransient<LikeLimiterService>();
builder.Services.AddTransient<IEmailSender, NoOpEmailSender>(); // This doesn't actually send an email.

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

await app.RunAsync();

public partial class Program { }
