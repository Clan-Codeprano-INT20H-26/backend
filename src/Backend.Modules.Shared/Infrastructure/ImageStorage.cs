using Backend.Modules.Shared.Interfaces.Image;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Error = FluentResults.Error;

namespace Backend.Modules.Shared.Infrastructure;

public class ImageStorage : IImageStorage
{
    private readonly Cloudinary _cloudinary;

    public ImageStorage(IConfiguration configuration)
    {
        var account = new Account(
            configuration["Cloudinary:CloudName"],
            configuration["Cloudinary:ApiKey"],
            configuration["Cloudinary:ApiSecret"]
        );

        _cloudinary = new Cloudinary(account);
        _cloudinary.Api.Secure = true;
    }

    public async Task<Result<string>> UploadAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return Result.Fail("File is empty or null");
        }

        try
        {
            using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "products_images", 
                // Transformation = new Transformation().Width(500).Height(500).Crop("fill") 
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null)
            {
                return Result.Fail(uploadResult.Error.Message);
            }
            
            return Result.Ok(uploadResult.SecureUrl.ToString());
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error("Cloudinary upload failed").CausedBy(ex));
        }
    }

    public async Task<Result> DeleteAsync(string publicId)
    {
        try
        {
            var deletionParams = new DeletionParams(publicId);
            var deletionResult = await _cloudinary.DestroyAsync(deletionParams);

            if (deletionResult.Result == "ok")
            {
                return Result.Ok();
            }
            
            return Result.Fail($"Failed to delete image. Cloudinary status: {deletionResult.Result}");
        }
        catch (Exception ex)
        {
             return Result.Fail(new Error("Cloudinary delete failed").CausedBy(ex));
        }
    }
}