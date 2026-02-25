using FluentResults;
using Microsoft.AspNetCore.Http;

namespace Backend.Modules.Shared.Interfaces.Image;

public interface IImageStorage
{
    Task<Result<string>> UploadAsync(IFormFile file);
    
    Task<Result> DeleteAsync(string publicId);
}