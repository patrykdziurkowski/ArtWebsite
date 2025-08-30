using System.Diagnostics;

namespace web.Features.Tags.ImageRecognition;

public class ImageTagger
{
        private const string ENGLISH_TAGS_LINE_START = "Image Tags: ";
        private readonly string _scriptLocation;
        private readonly string _pretrainedLocation;

        public ImageTagger()
        {
                string assemblyLocation = typeof(ImageTagger).Assembly.Location;

                // this works for both test and web assemblies
                string recognizeAnythingPath = Path.GetFullPath(Path.Combine(
                        assemblyLocation,
                        "..",
                        "..",
                        "..",
                        "..",
                        "..",
                        "web",
                        "Features",
                        "Tags",
                        "ImageRecognition",
                        "recognize-anything"));

                _scriptLocation = Path.GetFullPath(Path.Combine(
                        recognizeAnythingPath,
                        "inference_ram_plus.py"));

                _pretrainedLocation = Path.GetFullPath(Path.Combine(
                        recognizeAnythingPath,
                        "pretrained",
                        "ram_plus_swin_large_14m.pth"
                ));

                if (!File.Exists(_scriptLocation))
                {
                        throw new InvalidOperationException("The tagging script is not present in location: " + _scriptLocation);
                }

                if (!File.Exists(_pretrainedLocation))
                {
                        throw new InvalidOperationException("The tagging pretrained data is not present in location: " + _pretrainedLocation);
                }
        }

        public async Task<List<string>> TagImageAsync(string fullImagePath)
        {
                if (!File.Exists(fullImagePath))
                {
                        throw new InvalidOperationException("No image exists with the path: " + fullImagePath);
                }

                ProcessStartInfo processStartInfo = new()
                {
                        FileName = "python",
                        Arguments = $"{_scriptLocation} --image {fullImagePath} --pretrained {_pretrainedLocation}",
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                };

                using Process process = Process.Start(processStartInfo)
                        ?? throw new InvalidOperationException("Could not start the image tagging process.");

                string? line;
                while ((line = await process.StandardOutput.ReadLineAsync()) is not null)
                {
                        if (line.StartsWith(ENGLISH_TAGS_LINE_START))
                        {
                                await process.WaitForExitAsync();
                                return ExtractTags(line);
                        }
                }

                throw new InvalidOperationException("No output tags have been found while executing the image tagging process.");
        }

        private static List<string> ExtractTags(string taggingResult)
        {
                string delimitedTags = taggingResult[ENGLISH_TAGS_LINE_START.Length..];
                return [.. delimitedTags.Split(" | ")];
        }
}
