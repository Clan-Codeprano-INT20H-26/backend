using Backend.Modules.Order.Application.Interfaces;
using Backend.Modules.Order.Domain;
using Backend.Modules.Shared.Interfaces.Kit;
using Backend.Modules.Shared.Interfaces.Order;
using Backend.Modules.Shared.Interfaces.Tax;
using FluentResults;
using System.Globalization;
using Backend.Modules.Order.Application.Mappers;

namespace Backend.Modules.Order.Application;

public class OrderImportService : IOrderImportService
{
    private readonly ICsvParserService _csvParser;
    private readonly IOrderBulkRepository _bulkRepository;
    private readonly ITaxService _taxHelper;
    private readonly IKitService _kitService;

    public OrderImportService(
        ICsvParserService csvParser,
        IOrderBulkRepository bulkRepository,
        ITaxService taxHelper,
        IKitService kitService)
    {
        _csvParser = csvParser;
        _bulkRepository = bulkRepository;
        _taxHelper = taxHelper;
        _kitService = kitService;
    }

    public async Task<Result> ImportOrdersAsync(Guid userId, Stream fileStream, CancellationToken ct)
    {
        const int batchSize = 150;
        var batch = new List<Domain.Order>(batchSize);

        try
        {
            await foreach (var dto in _csvParser.ReadOrdersStreamAsync(fileStream, ct))
            {
                if (!decimal.TryParse(dto.Latitude, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal lat) ||
                    !decimal.TryParse(dto.Longitude, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal lon))
                {
                    Console.WriteLine($"[IMPORT] Invalid coordinates");
                    continue;
                }
    
                var kitResult = await _kitService.CalculateTotalPriceAsync(dto.Items);
                
                if (kitResult.IsFailed)
                {
                    Console.WriteLine($"[IMPORT] Kit failed: {kitResult.Errors.FirstOrDefault()?.Message}");
                    continue;
                }

                var taxResult = await _taxHelper.CalculateTaxesAsync(lat, lon);
                if (taxResult.IsFailed)
                {
                    Console.WriteLine($"[IMPORT] Tax failed: {taxResult.Errors.FirstOrDefault()?.Message}");
                    continue;
                }
                
                var domainItems = OrderItemMapper.ToDomains(dto.Items);
                
                var newOrder = new Domain.Order(
                    userId, 
                    domainItems,  
                    kitResult.Value, 
                    dto.Latitude, 
                    dto.Longitude
                );

                var taxDto = taxResult.Value;
                newOrder.ApplyTax(new TaxesBreakdown
                {
                    StateRate = taxDto.StateRate,
                    CountryRate = taxDto.CountyRate,
                    CityRate = taxDto.CityRate,
                    SpecialRates = taxDto.SpecialRates,
                    Jurisdictions = new List<string>(taxDto.Jurisdictions) ?? new List<string>()
                });

                batch.Add(newOrder);

                if (batch.Count >= batchSize)
                {
                    await _bulkRepository.BulkInsertOrdersAsync(batch, ct);
                    batch.Clear();
                }
            }

            if (batch.Any())
            {
                await _bulkRepository.BulkInsertOrdersAsync(batch, ct);
            }

            return Result.Ok();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[IMPORT] Exception: {ex.Message}");
            return Result.Fail(new Error("Import failed").CausedBy(ex));
        }
    }
}