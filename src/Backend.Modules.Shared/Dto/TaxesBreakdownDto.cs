namespace Backend.Modules.Shared.Dto;

public record TaxesBreakdownDto(
    decimal StateRate,
    decimal CountryRate,
    decimal CityRate,
    decimal SpecialRates
);