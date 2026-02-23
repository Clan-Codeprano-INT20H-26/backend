using System.Net.Http.Json;
using Backend.Modules.Order.Application.Interfaces;
using Backend.Modules.Order.Domain;
using Microsoft.Extensions.Configuration;

namespace Backend.Modules.Order.Infrastructure;

public class TaxHelper : ITaxHelper
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public TaxHelper(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClient = httpClientFactory.CreateClient();
        _apiKey = configuration["ZipTax:ApiKey"] ?? "YOUR_KEY";
    }

    public async Task<TaxesBreakdown> GetTaxesBreakdownAsync(string latitude, string longitude)
    {
        var url = $"https://api.zip-tax.com/request/v60?key={_apiKey}&lat={latitude}&lng={longitude}&format=json";

        try 
        {
            var apiResponse = await _httpClient.GetFromJsonAsync<ZipTaxResponse>(url);

            if (apiResponse?.BaseRates == null) 
                return new TaxesBreakdown();

            var salesTaxes = apiResponse.BaseRates
                .Where(r => r.JurType.Contains("SALES_TAX"))
                .ToList();

            return new TaxesBreakdown
            {
                StateRate = salesTaxes
                    .Where(r => r.JurType == "US_STATE_SALES_TAX")
                    .Sum(r => r.Rate),

                CountryRate = salesTaxes
                    .Where(r => r.JurType == "US_COUNTY_SALES_TAX")
                    .Sum(r => r.Rate),

                CityRate = salesTaxes
                    .Where(r => r.JurType == "US_CITY_SALES_TAX")
                    .Sum(r => r.Rate),

                SpecialRates = salesTaxes
                    .Where(r => r.JurType == "US_DISTRICT_SALES_TAX")
                    .Sum(r => r.Rate),

                Jurisdictions = salesTaxes
                    .Select(r => r.JurName)
                    .Distinct()
                    .ToList()
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching taxes: {ex.Message}");
            return new TaxesBreakdown(); 
        }
    }
    public class ZipTaxResponse
    {
        public List<ZipTaxRate> BaseRates { get; set; } = new();
    }

    public class ZipTaxRate
    {
        public decimal Rate { get; set; }
        public string JurType { get; set; } = string.Empty; 
        public string JurName { get; set; } = string.Empty; 
    }
}