using System.ComponentModel.DataAnnotations;
using Microsoft.IdentityModel.Tokens;

namespace web.Features.Images;

public class SupportedImageAttribute : ValidationAttribute
{
        public static readonly string[] SupportedImageExtensions = [".png", ".jpg", ".jpeg"];

        public override bool IsValid(object? value)
        {
                if (value is not IFormFile)
                {
                        ErrorMessage = "Given value is not a file.";
                        return false;
                }

                IFormFile formFile = (IFormFile)value;
                if (formFile.FileName.IsNullOrEmpty())
                {
                        ErrorMessage = "Given file's name should not be empty.";
                        return false;
                }

                string? extension = Path.GetExtension(formFile.FileName);
                if (extension is null)
                {
                        ErrorMessage = "Given value has no type.";
                        return false;
                }

                if (SupportedImageExtensions.Contains(extension) == false)
                {
                        ErrorMessage = $"Given file is not of supported type: {string.Join(" ", SupportedImageExtensions)}";
                        return false;
                }
                return true;
        }
}
