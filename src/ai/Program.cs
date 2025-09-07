using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapPost("/tag", async (HttpContext context) =>
{
        if (!context.Request.HasFormContentType)
        {
                return Results.BadRequest("Expected form data with a file.");
        }

        IFormCollection form = await context.Request.ReadFormAsync();
        IFormFile? file = form.Files["image"];
        if (file is null)
        {
                return Results.BadRequest("No image uploaded.");
        }

        if (file.Length <= 0)
        {
                return Results.BadRequest("Empty image uploaded.");
        }

        string imagePath = Path.Combine("/app/images", Guid.NewGuid().ToString());
        Directory.CreateDirectory("/app/images");
        using Stream fileStream = new FileStream(imagePath, FileMode.CreateNew);
        await file.CopyToAsync(fileStream);

        Process tagProcess = new()
        {
                StartInfo = new()
                {
                        RedirectStandardOutput = true,
                        FileName = "python",
                        Arguments = $"inference_ram_plus.py --image {imagePath} --pretrained pretrained/ram_plus_swin_large_14m.pth",
                }
        };
        tagProcess.Start();
        await tagProcess.WaitForExitAsync();

        const string ENGLISH_TAGS_LINE_START = "Image Tags: ";
        string? line;
        while ((line = await tagProcess.StandardOutput.ReadLineAsync()) is not null)
        {
                Console.WriteLine(line);
                if (line.StartsWith(ENGLISH_TAGS_LINE_START))
                {
                        List<string> tags = [.. line[ENGLISH_TAGS_LINE_START.Length..].Split(" | ")];
                        File.Delete(imagePath);
                        return Results.Json(tags);
                }
        }

        File.Delete(imagePath);
        return Results.InternalServerError("Could not find the tags for this image.");
});

app.Run();

