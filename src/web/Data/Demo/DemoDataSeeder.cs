using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using web.Features.Artists;
using web.Features.Artists.BoostArtPiece;
using web.Features.ArtPieces;
using web.Features.ArtPieces.UploadArtPiece;
using web.Features.Browse;
using web.Features.Reviewers;
using web.Features.Reviewers.LikeArtPiece;
using web.Features.Reviews.ReviewArtPiece;

namespace web.Data.Demo;

/// <summary>
/// Meant for demo/development use only. Seeds the app with example data
/// for presentation purposes.
/// </summary>
public class DemoDataSeeder(
    IHostEnvironment environment,
    ApplicationDbContext dbContext,
    UploadArtPieceCommand uploadArtPieceCommand,
    ReviewArtPieceCommand reviewArtPieceCommand,
    RegisterArtPieceServedCommand registerArtPieceServedCommand,
    BoostArtPieceCommand boostArtPieceCommand,
    LikeArtPieceCommand likeArtPieceCommand,
    UserManager<IdentityUser<Guid>> userManager)
{
    private readonly Random _random = new();

    private readonly string[] _reviewerNames = [
        "ArtLover2024", "PixelSeeker", "DigitalDreamer", "ColorTheory", "VisualVoyager",
        "CreativeSoul88", "DesignDiver", "StyleHunter", "AestheticAddict", "GalleryGazer",
        "CanvasCritic", "MuseHunter", "ArtisticEye", "CultureVulture", "VisualVibes",
        "InspiredDaily", "CreativeCorner", "ArtExplorer", "DesignEnthusiast", "VibrantViews",
        "ModernMuse", "AbstractLover", "ColorPalette", "ArtWatcher", "SketchFan",
        "DigitalNomad", "CuriousCollector", "TrendSpotter", "StyleSavvy", "CreativeMinds",
        "ArtisticJourney", "DesignDaily", "VisualStories", "ConceptCritic", "ColorfulThoughts",
        "ArtAppreciator", "PixelPusher", "DesignGeek", "CreativeVibes", "GalleryHopper",
        "ModernArtFan", "SketchLover", "DigitalEyes", "CritiqueQueen", "ArtObserver",
        "StyleWatch", "DesignDiary", "CreativeFlow", "VisualNotes", "ArtAndSoul",
    ];

    private readonly string[] _artistNames = [
        "MidnightCanvas", "NeonDreams", "CrimsonBrush", "EchoStudios", "VoidArtworks",
        "PixelSage", "LunarCreates", "ShadowCraft", "CosmicPalette", "VelvetVisuals",
        "StormDesigns", "CrystalLens", "PhoenixArt", "AzureInk", "FrostPixels",
        "ZenDraws", "OceanicArt", "EmberStudio", "QuantumDesign", "NovaSketches",
        "SilentMuse", "RavenWorks", "CelestialArts", "IronPixel", "MysticBrush",
    ];
    
    private readonly string[] _artistStyles = [
        "Digital illustrator specializing in character design and concept art.",
        "Abstract painter exploring color theory and emotional expression.",
        "3D artist creating surreal environments and architectural visualizations.",
        "Traditional watercolor artist inspired by nature and landscapes.",
        "Street artist bringing urban stories to life through murals.",
        "Minimalist designer focused on clean lines and negative space.",
        "Fantasy artist crafting otherworldly creatures and magical realms.",
        "Portrait artist capturing the essence of human emotion.",
        "Experimental mixed media artist pushing creative boundaries.",
        "Comic book artist with a passion for dynamic storytelling.",
        "Pixel art creator inspired by retro gaming aesthetics.",
        "Sculptor working with recycled materials and found objects.",
        "Fashion illustrator documenting trends and textile designs.",
        "Botanical artist celebrating the beauty of plants and flowers.",
        "Cyberpunk artist exploring futuristic dystopian themes.",
        "Children's book illustrator creating whimsical characters.",
        "Landscape photographer turned digital painter.",
        "Anime-inspired artist with a focus on dynamic compositions.",
        "Surrealist exploring the subconscious through dreamlike imagery.",
        "Contemporary artist examining social issues through visual art.",
    ];
    
    private readonly string[] _artDescriptions = [
        "Exploring the intersection of light and shadow",
        "A study in composition and balance",
        "Capturing fleeting moments in time",
        "Experimenting with new techniques",
        "Part of my ongoing series",
        "Inspired by recent travels",
        "A personal reflection on growth",
        "Playing with color and texture",
        "My interpretation of a classic theme",
        "Pushing my creative boundaries",
    ];

    private readonly string[] _reviewComments = [
        "This piece really captures the essence of modern digital art. The color palette is stunning and the composition draws you in immediately.",
        "I love the emotional depth in this work. The artist's technique is masterful and the attention to detail is incredible.",
        "Absolutely breathtaking! The way light and shadow interact here is phenomenal. This deserves to be in a gallery.",
        "The textures and layering in this piece are so well executed. You can tell the artist put a lot of thought into every element.",
        "This speaks to me on a personal level. The symbolism is powerful and the execution is flawless. Beautiful work.",
        "Really interesting interpretation of the theme. The style is unique and memorable. I'd love to see more from this artist.",
        "The composition is dynamic and engaging. My eyes keep finding new details every time I look at it. Fantastic work!",
        "Such a creative approach to a classic subject. The artist's vision really shines through in this piece. Very impressive.",
        "The mood and atmosphere are perfectly captured here. This piece tells a story without words. Truly remarkable.",
        "Bold color choices that really work together harmoniously. The contrast creates such visual interest. Love this!",
        "This demonstrates excellent technical skill combined with artistic vision. The balance and flow are just right.",
        "I appreciate the thoughtfulness behind this work. Every element serves a purpose. This is art with intention.",
        "The energy in this piece is infectious! The movement and rhythm create such a captivating visual experience.",
        "Stunning use of negative space and form. The minimalist approach works perfectly here. Less is definitely more.",
        "This piece challenges conventions in the best way. It's thought-provoking and visually striking. A true standout.",
    ];

    private readonly List<ArtPieceId> _artPieceIds = [];

    public async Task ExecuteAsync()
    {
        if (!environment.IsDevelopment())
        {
            throw new InvalidOperationException("Attempted to seed demo data into a non-development application build.");
        }

        for (int i = 0; i < 25; i++)
        {
            await CreateArtist(i);
        }

        for (int i = 0; i < 50; i++)
        {
            await CreateReviewer(i);
        }
    }

    private async Task CreateArtist(int index)
    {
        string artistName = _artistNames[index];

        IdentityUser<Guid> user = new(artistName);
        await userManager.CreateAsync(user, "Password123!");
        
        dbContext.Reviewers.Add(new Reviewer
        {
            Name = artistName,
            UserId = user.Id,
        });
        
        ArtistId artistId = new();
        dbContext.Artists.Add(new Artist
        {
            Id = artistId,
            UserId = user.Id,
            Name = artistName,
            Summary = _artistStyles[index % _artistStyles.Length],
        });

        await dbContext.SaveChangesAsync();
        
        // 0 - 4 art pieces per artist
        int artPieceCount = index % 5;
        for (int i = 0; i < artPieceCount; i++)
        {
            ArtPiece artPiece = await uploadArtPieceCommand.ExecuteAsync(
                GetRandomImage(),
                _artDescriptions[index % _artDescriptions.Length],
                user.Id,
                now: GetRandomDate());
            _artPieceIds.Add(artPiece.Id);

            // 50/50 chance to boost first art piece
            if (i == 0 && _random.Next(0, 2) == 0)
            {
                await boostArtPieceCommand.ExecuteAsync(
                    user.Id,
                    artPiece.Id,
                    GetRandomDate(from: artPiece.UploadDate));
            } 
        }
    }

    private async Task CreateReviewer(int index)
    {           
        List<ArtPieceId> reviewedArtPieceIds = [];
        string reviewerName = _reviewerNames[index];

        IdentityUser<Guid> user = new(reviewerName);
        await userManager.CreateAsync(user, "Password123!");
        
        dbContext.Reviewers.Add(new Reviewer
        {
            Name = reviewerName,
            UserId = user.Id,
        });

        await dbContext.SaveChangesAsync();

        int reviewCount = index % 12; // 0 - 11 reviews
        for (int i = 0; i < reviewCount; i++)
        {
            List<ArtPieceId> artPieceIdsLeftToReview = [.. _artPieceIds
                .Where(id => !reviewedArtPieceIds.Contains(id))];
            ArtPieceId artPieceId = artPieceIdsLeftToReview[_random.Next(0, artPieceIdsLeftToReview.Count)];
            ArtPiece artPiece = await dbContext.ArtPieces.FirstAsync(ap => ap.Id == artPieceId);
            DateTimeOffset reviewDate = GetRandomDate(from: artPiece.UploadDate);

            await registerArtPieceServedCommand.ExecuteAsync(
                user.Id,
                artPieceId,
                reviewDate);
            await reviewArtPieceCommand.ExecuteAsync(
                comment: _reviewComments[_random.Next(0, _reviewComments.Length)],
                rating: _random.Next(1, 6),
                artPieceId,
                user.Id,
                TimeSpan.FromSeconds(0),
                reviewDate);
            reviewedArtPieceIds.Add(artPieceId);
            await registerArtPieceServedCommand.ExecuteAsync(user.Id, null);
            
            // 50/50 chance to like each of the first 5 art pieces
            if (i < 5 && _random.Next(0, 2) == 1)
            {
                await likeArtPieceCommand.ExecuteAsync(user.Id, artPieceId, reviewDate.AddSeconds(5));
            }
        }
    }

    private IFormFile GetRandomImage()
    {
        string imagesPath = "Data/Demo/";

        int imageNumber = _random.Next(1, 6);
        string imagePath = Path.Combine(imagesPath, $"sample-image-{imageNumber}.jpg");
        
        if (!File.Exists(imagePath))
        {
            throw new FileNotFoundException($"Sample image not found: {imagePath}");
        }
        
        FileStream stream = File.OpenRead(imagePath);
        return new FormFile(stream, 0, stream.Length, "file", Path.GetFileName(imagePath))
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/jpeg"
        };
    }

    private DateTimeOffset GetRandomDate(DateTimeOffset? from = null)
    {
        DateTimeOffset start = from ?? new(2022, 1, 1, 0, 0, 0, TimeSpan.Zero);
        DateTimeOffset end = DateTimeOffset.UtcNow;
        
        int rangeDays = (int)(end - start).TotalDays;
        int randomDays = _random.Next(rangeDays);
        
        return start.AddDays(randomDays);
    }
}
