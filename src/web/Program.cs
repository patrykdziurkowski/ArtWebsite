using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using web.data;
using web.features.artist.DeactivateArtist;
using web.features.artist.SetupArtist;
using web.features.shared;
using web.Features.ArtPiece.Index;
using web.Features.ArtPiece.UploadArtPiece;

var builder = WebApplication.CreateBuilder(args);
string connectionString = builder.Configuration["CONNECTION_STRING"]
                ?? throw new InvalidOperationException("Connection string not found.");
builder.Services
        .AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
builder.Services
        .AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();
builder.Services.AddControllersWithViews(o =>
{
        o.Filters.Add<ValidateModelFilter>();
});
builder.Services.Configure<RazorViewEngineOptions>(o =>
{
        /*
            {2} - area name
            {1} - controller name
            {0} - action name
        */
        o.ViewLocationFormats.Clear();
        o.ViewLocationFormats.Add("/features/{2}/{0}.cshtml");
        o.ViewLocationFormats.Add("/features/{2}/{1}/{0}.cshtml");
        o.ViewLocationFormats.Add("/features/{1}/{0}{1}/{0}.cshtml");
        o.ViewLocationFormats.Add("/features/{1}/{0}/{0}.cshtml");
        o.ViewLocationFormats.Add("/features/shared/{0}.cshtml");

        o.AreaPageViewLocationFormats.Clear();
        o.AreaPageViewLocationFormats.Add("/features/authentication/{2}/{0}.cshtml");
        o.AreaPageViewLocationFormats.Add("/features/authentication/{2}/{1}/{0}.cshtml");
        o.AreaPageViewLocationFormats.Add("/features/authentication/{0}.cshtml");
        o.AreaPageViewLocationFormats.Add("/features/shared/{0}.cshtml");
});

builder.Services.AddTransient<SetupArtistCommand>();
builder.Services.AddTransient<DeactivateArtistCommand>();
builder.Services.AddTransient<UploadArtPieceCommand>();
builder.Services.AddTransient<ArtPiecesQuery>();


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
        RoleManager<IdentityRole> roleManager = scope.ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole>>();
        string[] roles = ["Admin", "Artist"];
        foreach (string role in roles)
        {
                if (await roleManager.RoleExistsAsync(role) == false)
                {
                        await roleManager.CreateAsync(new IdentityRole(role));
                }
        }
}

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
