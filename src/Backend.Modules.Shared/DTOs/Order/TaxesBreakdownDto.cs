namespace Backend.Modules.Shared.DTOs.Order;

public record TaxesBreakdownDto(
    decimal StateRate,
    decimal CountryRate,
    decimal CityRate,
    decimal SpecialRates
);