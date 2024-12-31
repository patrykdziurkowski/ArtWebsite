using System;
using FluentResults;
using web.data;
using web.features.artist;

namespace web.Features.ArtPiece.UploadArtPiece;

public class UploadArtPieceCommand
{
        private readonly ApplicationDbContext _dbContext;
        private readonly IWebHostEnvironment _env;

        public UploadArtPieceCommand(ApplicationDbContext dbContext,
                IWebHostEnvironment env)
        {
                _dbContext = dbContext;
                _env = env;
        }

        public async Task<ArtPiece> ExecuteAsync(IFormFile file,
                string description, ArtistId artistId)
        {
                string directoryPath = Path.Combine(_env.ContentRootPath, "wwwroot",
                        "images", "art-pieces", $"{artistId}");
                Directory.CreateDirectory(directoryPath);

                ArtPieceId artPieceId = new();
                string filePath = Path.Combine(directoryPath, $"{artPieceId}");
                using (FileStream stream = new(filePath, FileMode.Create))
                {
                        await file.CopyToAsync(stream);
                }

                ArtPiece artPiece = new(artPieceId, filePath,
                        description, DateTime.UtcNow, artistId);
                await _dbContext.ArtPieces.AddAsync(artPiece);
                await _dbContext.SaveChangesAsync();
                return artPiece;
        }
}
