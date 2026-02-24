using Backend.Module.Kit.Application.Mapper;
using Backend.Module.Kit.Infrastructure;
using Backend.Modules.Shared.DTOs.Kit;
using Backend.Modules.Shared.Interfaces.Kit;
using FluentResults;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;


namespace Backend.Module.Kit.Application;

public class KitService : IKitService
{
    private readonly KitDbContext _context;
    private readonly IWebHostEnvironment _env;

    public KitService(KitDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public async Task<Result<List<KitResponse>>> GetAllAsync()
    {
        try
        {
            var kits = await _context.Kits.AsNoTracking().ToListAsync();
            return Result.Ok(kits.ToResponseList());
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error("Failed to retrieve kits").CausedBy(ex));
        }
    }

    public async Task<Result<KitResponse>> GetByIdAsync(Guid id)
    {
        var kit = await _context.Kits.FindAsync(id);

        if (kit == null)
            return Result.Fail($"Kit with ID {id} not found");

        return Result.Ok(kit.ToResponse());
    }

    public async Task<Result<KitResponse>> CreateAsync(CreateKitDto dto)
    {
        if (dto.price < 0)
            return Result.Fail("Price cannot be negative");

        try
        {
            //var imagePaths = await SaveFilesAsync(dto.images); TODO Image upload

            var kit = new Domain.Kit
            {
                Id = Guid.NewGuid(),
                Name = dto.name,
                Description = dto.description,
                Seller = dto.seller,
                Price = dto.price,
                Images = new List<string>()// TODO imporve
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

    public async Task<Result<KitResponse>> UpdateAsync(Guid id, UpdateKitDto dto)
    {
        var kit = await _context.Kits.FirstOrDefaultAsync(k => k.Id == id);
        
        if (kit == null)
            return Result.Fail($"Kit with ID {id} not found");

        if (!string.IsNullOrWhiteSpace(dto.name)) kit.Name = dto.name;
        if (!string.IsNullOrWhiteSpace(dto.description)) kit.Description = dto.description;
        if (dto.price.HasValue) kit.Price = dto.price.Value;
        
        if (dto.newImages != null && dto.newImages.Any())
        {
            try
            {
                //kit.Images = await SaveFilesAsync(dto.NewImages);
            }
            catch (Exception ex)
            {
                return Result.Fail(new Error("Failed to save new images").CausedBy(ex));
            }
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

        var total = kits.Sum(x => x.price);
        
        return Result.Ok(total);
    }
    public async Task<Result<decimal>> CalculateTotalPriceAsync(IEnumerable<Guid> kitIds)
    {
        if (kitIds == null)
            return Result.Fail("List of IDs cannot be null");

        try
        {
            var total = await _context.Kits
                .Where(k => kitIds.Contains(k.Id))
                .SumAsync(k => k.Price);

            return Result.Ok(total);
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error("Failed to calculate total from DB").CausedBy(ex));
        }
    }
    
}