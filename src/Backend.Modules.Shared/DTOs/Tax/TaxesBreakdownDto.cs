namespace Backend.Modules.Shared.DTOs.Tax;

public record TaxesBreakdownDto(
    decimal StateRate,
    decimal CountyRate,
    decimal CityRate,
    decimal SpecialRates,
    List<string> Jurisdictions
);