namespace web.Features.Images;

public class ImageManager
{
        public async Task<string> SaveOrUpdateImageAsync(IFormFile file, string pathToFolder, string fileName)
        {
                string directoryPath = Path.Combine("user-images", pathToFolder.TrimStart(Path.DirectorySeparatorChar));
                Directory.CreateDirectory(directoryPath);
                string fileExtension = Path.GetExtension(file.FileName);
                string imagePath = Path.Combine(directoryPath, $"{fileName}{fileExtension}");
                using (FileStream stream = new(imagePath, FileMode.Create, FileAccess.Write))
                {
                        await file.CopyToAsync(stream);
                }

                // the slash here is important in order for browser to use absolute image src
                return '/' + imagePath;
        }
}
