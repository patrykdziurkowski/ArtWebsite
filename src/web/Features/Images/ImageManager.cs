using web.Features.Artists;
using web.Features.ArtPieces;
using web.Features.Reviewers;

namespace web.Features.Images;

public class ImageManager
{
        public Task<string> UpdateReviewerProfilePictureAsync(IFormFile file, ReviewerId reviewerId)
        {
                return SaveOrUpdateImageAsync(file, "profile-pictures/reviewers", reviewerId.ToString());
        }

        public Task<string> UpdateArtistProfilePictureAsync(IFormFile file, ArtistId artistId)
        {
                return SaveOrUpdateImageAsync(file, "profile-pictures/artists", artistId.ToString());
        }

        public Task<string> SaveArtPieceImageAsync(IFormFile file, ArtistId artistId, ArtPieceId artPieceId)
        {
                return SaveOrUpdateImageAsync(file, $"art-pieces/{artistId}", artPieceId.ToString());
        }

        private static async Task<string> SaveOrUpdateImageAsync(IFormFile file, string pathToFolder, string fileName)
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
