using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var labels = File.ReadAllLines("models/synset.txt");

app.MapPost("/tag", async (HttpContext context) =>
{
        var file = context.Request.Form.Files["image"];
        if (file is null)
        {
                return Results.BadRequest("No image provided");
        }

        using var stream = file.OpenReadStream();
        try
        {
                using var image = await Image.LoadAsync<Rgb24>(stream);

                image.Mutate(x => x.Resize(new ResizeOptions
                {
                        Size = new Size(224, 224),
                        Mode = ResizeMode.Crop
                }));

                float[] mean = [0.485f, 0.456f, 0.406f];
                float[] stddev = [0.229f, 0.224f, 0.225f];
                DenseTensor<float> processedImage = new([1, 3, 224, 224]);
                image.ProcessPixelRows(accessor =>
                {
                        for (int y = 0; y < accessor.Height; y++)
                        {
                                Span<Rgb24> pixelSpan = accessor.GetRowSpan(y);
                                for (int x = 0; x < accessor.Width; x++)
                                {
                                        processedImage[0, 0, y, x] = ((pixelSpan[x].R / 255f) - mean[0]) / stddev[0];
                                        processedImage[0, 1, y, x] = ((pixelSpan[x].G / 255f) - mean[1]) / stddev[1];
                                        processedImage[0, 2, y, x] = ((pixelSpan[x].B / 255f) - mean[2]) / stddev[2];
                                }
                        }
                });


                using var inputOrtValue = OrtValue.CreateTensorValueFromMemory(OrtMemoryInfo.DefaultInstance,
                        processedImage.Buffer, [1, 3, 224, 224]);

                var inputs = new Dictionary<string, OrtValue>
                        {
                                { "data", inputOrtValue }
                        };

                using var session = new InferenceSession("models/vgg16-12-qdq.onnx");
                using var runOptions = new RunOptions();
                using IDisposableReadOnlyCollection<OrtValue> results = session.Run(runOptions, inputs, session.OutputNames);

                var output = results[0].GetTensorDataAsSpan<float>().ToArray();
                float sum = output.Sum(x => (float)Math.Exp(x));
                IEnumerable<float> softmax = output.Select(x => (float)Math.Exp(x) / sum);

                IEnumerable<string> top10 = softmax.Select((x, i) => new Prediction { Label = labels[i], Confidence = x })
                        .OrderByDescending(x => x.Confidence)
                        .Select(p => p.Label)
                        .Select(label => label.Replace(",", "").Split(' ')[1])
                        .Take(10);

                return Results.Json(top10);
        }
        catch (UnknownImageFormatException)
        {
                return Results.BadRequest("Unrecognized image format");
        }
});

app.MapGet("/health", () => Results.Ok("Healthy"));

app.Run();

public record Prediction
{
        public required string Label { get; init; }
        public required float Confidence { get; init; }
}



