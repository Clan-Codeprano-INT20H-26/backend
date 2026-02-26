namespace Backend.Modules.Shared.DTOs.Tax;

public record TaxBreakdownResponse(
    decimal StateRate,
    decimal CountyRate,
    decimal CityRate,
    decimal SpecialRates,
    IReadOnlyList<string> Jurisdictions
);