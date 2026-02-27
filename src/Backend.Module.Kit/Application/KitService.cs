using Backend.Module.Kit.Application.Mapper;
using Backend.Module.Kit.Infrastructure;
using Backend.Modules.Shared.DTOs.Kit;
using Backend.Modules.Shared.DTOs.Order;
using Backend.Modules.Shared.DTOs.Pagination;
using Backend.Modules.Shared.Interfaces.Kit;
using Backend.Modules.Shared.Interfaces.Image;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Backend.Module.Kit.Application;

public class KitService : IKitService
{
    private readonly KitDbContext _context;
    private readonly IImageStorage _imageStorage; 

    public KitService(KitDbContext context, IImageStorage imageStorage)
    {
        _context = context;
        _imageStorage = imageStorage;
    }

    public async Task<Result<PagedResponse<KitResponse>>> GetAllAsync(KitFilterRequest filter)
    {
        try
        {
            var query = _context.Kits.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var term = filter.SearchTerm.ToLower();
                query = query.Where(k => 
                    EF.Functions.TrigramsSimilarity(k.Name, term) > 0.2 || 
                    EF.Functions.TrigramsSimilarity(k.Description, term) > 0.2);
            }

            if (!string.IsNullOrWhiteSpace(filter.Seller))
            {
                query = query.Where(k => k.Seller.ToLower().Contains(filter.Seller.ToLower()));
            }

            if (filter.MinPrice.HasValue)
            {
                query = query.Where(k => k.Price >= filter.MinPrice.Value);
            }

            if (filter.MaxPrice.HasValue)
            {
                query = query.Where(k => k.Price <= filter.MaxPrice.Value);
            }
            var totalCount = await query.CountAsync();
            
            var sortBy = filter.SortBy?.ToLower()?.Trim();
            var isDesc = filter.IsDescending;
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm) && sortBy == "relevance")
            {
                var term = filter.SearchTerm.Trim();
                query = query.OrderByDescending(k => 
                    Math.Max(
                        EF.Functions.TrigramsSimilarity(k.Name, term), 
                        EF.Functions.TrigramsSimilarity(k.Description, term)
                    ));
            }
            else
            {
                query = sortBy switch
                {
                    "price" => isDesc ? query.OrderByDescending(k => k.Price) : query.OrderBy(k => k.Price),
                    "seller" => isDesc ? query.OrderByDescending(k => k.Seller) : query.OrderBy(k => k.Seller),
                    "name" => isDesc ? query.OrderByDescending(k => k.Name) : query.OrderBy(k => k.Name),
                    _ => query.OrderBy(k => k.Name) 
                };
            }
            
            

            var pageNumber = filter.PageNumber < 1 ? 1 : filter.PageNumber;
            var pageSize = filter.PageSize < 1 ? 10 : filter.PageSize;
            
            var kits = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var dtos = kits.ToResponseList(); 

            var result = new PagedResponse<KitResponse>(dtos, totalCount, pageNumber, pageSize);
            
            return Result.Ok(result);
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error("Failed to retrieve kits").CausedBy(ex));
        }
    }

    public async Task<Result<KitResponse>> GetByIdAsync(Guid id)
    {
        try
        {
            var kit = await _context.Kits.FindAsync(id);

            if (kit == null)
                return Result.Fail($"Kit with ID {id} not found");

            return Result.Ok(kit.ToResponse());
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error("Database error").CausedBy(ex));
        }
    }

    public async Task<Result<KitResponse>> CreateAsync(CreateKitRequest request)
    {
        if (request.Price < 0)
            return Result.Fail("Price cannot be negative");

        List<string> uploadedImageUrls = new();
        if (request.Images != null && request.Images.Any())
        {
            var uploadResult = await UploadFilesInternalAsync(request.Images);
            if (uploadResult.IsFailed)
            {
                return Result.Fail(uploadResult.Errors);
            }
            uploadedImageUrls = uploadResult.Value;
        }

        try
        {
            var kit = new Domain.Kit
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                Seller = request.Seller,
                Price = request.Price,
                Images = uploadedImageUrls 
            };

            _context.Kits.Add(kit);
            await _context.SaveChangesAsync();

            return Result.Ok(kit.ToResponse());
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error("Failed to create kit").CausedBy(ex));
        }
    }

    public async Task<Result<KitResponse>> UpdateAsync(Guid id, UpdateKitRequest request)
    {
        var kit = await _context.Kits.FirstOrDefaultAsync(k => k.Id == id);
        
        if (kit == null)
            return Result.Fail($"Kit with ID {id} not found");
        
        if (!string.IsNullOrWhiteSpace(request.Name)) kit.Name = request.Name;
        if (!string.IsNullOrWhiteSpace(request.Description)) kit.Description = request.Description;
        if (request.Price.HasValue) kit.Price = request.Price.Value;
        
        if (request.NewImages != null && request.NewImages.Any())
        {
            var uploadResult = await UploadFilesInternalAsync(request.NewImages);
            
            if (uploadResult.IsFailed)
            {
                return Result.Fail(uploadResult.Errors);
            }
            kit.Images.AddRange(uploadResult.Value);
            
            _context.Entry(kit).Property(k => k.Images).IsModified = true;
        }

        try
        {
            await _context.SaveChangesAsync();
            return Result.Ok(kit.ToResponse());
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error("Database error while updating").CausedBy(ex));
        }
    }
    
    public async Task<Result> DeleteAsync(Guid id)
    {
        var kit = await _context.Kits.FindAsync(id);
        
        if (kit == null)
            return Result.Fail($"Kit with ID {id} not found");

        try
        {
            
            _context.Kits.Remove(kit);
            await _context.SaveChangesAsync();
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error("Failed to delete kit").CausedBy(ex));
        }
    }

    public Result<decimal> CalculateTotalPrice(List<KitResponse> kits)
    {
        if (kits == null)
            return Result.Fail("Kits list cannot be null");

        var total = kits.Sum(x => x.Price);
        
        return Result.Ok(total);
    }

    public async Task<Result<decimal>> CalculateTotalPriceAsync(IEnumerable<CreateOrderItemRequest> kitPackDtos)
    {
        var orderItemDtos = kitPackDtos.ToList();
        if (kitPackDtos == null || !orderItemDtos.Any())
            return Result.Ok(0m);

        try
        {
            var uniqueKitIds = orderItemDtos.Select(x => x.KitId).Distinct().ToList();
        

            var foundKits = await _context.Kits
                .AsNoTracking() 
                .Where(k => uniqueKitIds.Contains(k.Id))
                .Select(k => new { k.Id, k.Price })
                .ToListAsync();
        

            if (foundKits.Count != uniqueKitIds.Count)
            {
                var foundIds = foundKits.Select(k => k.Id).ToHashSet();
                var missingIds = uniqueKitIds.Where(id => !foundIds.Contains(id));
        
                return Result.Fail($"Kits not found: {string.Join(", ", missingIds)}");
            }

            var priceMap = foundKits.ToDictionary(k => k.Id, k => k.Price);

            var total = orderItemDtos.Sum(dto => priceMap[dto.KitId] * dto.Quantity);

            return Result.Ok(total);
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error("Failed to calculate total from DB").CausedBy(ex));
        }
    }

    private async Task<Result<List<string>>> UploadFilesInternalAsync(List<IFormFile> files)
    {
        var uploadedUrls = new List<string>();

        foreach (var file in files)
        {
            var result = await _imageStorage.UploadAsync(file);

            if (result.IsFailed)
            {
                return Result.Fail(result.Errors);
            }

            uploadedUrls.Add(result.Value);
        }

        return Result.Ok(uploadedUrls);
    }
}