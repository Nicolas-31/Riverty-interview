using _1c.Data;
using _1c.Models;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Net.Http;
using System.Text.Json;

namespace _1c
{
    internal class Program
    {
        private const string ApiKey = "abd0a86748e44f2095d949529393812a";
        private static readonly HttpClient _httpClient = new HttpClient();
        static async Task Main(string[] args)
        {
            // My database set up
            using var context = new ExchangeRateContext();

            // Fetch the latest rates
            var latestRates = await FetchLatestRates();

            // Store the latest rates in the database
            foreach (var rate in latestRates)
            {
                var exchangeRate = new ExchangeRate
                {
                    Date = DateTime.UtcNow,
                    FromCurrency = "EUR", // Fixer.io uses EUR as the base for free accounts
                    ToCurrency = rate.Key,
                    Rate = rate.Value
                };

                // Add to the database context
                context.ExchangeRates.Add(exchangeRate);
            }

            // Save changes to the database
            await context.SaveChangesAsync();

            Console.WriteLine("Latest exchange rates updated in the database.");
        }

        private static async Task<Dictionary<string, decimal>> FetchLatestRates()
        {
            string url = $"http://data.fixer.io/api/latest?access_key={ApiKey}";
            HttpResponseMessage response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            using JsonDocument document = JsonDocument.Parse(responseBody);
            JsonElement root = document.RootElement;
            JsonElement rates = root.GetProperty("rates");

            var latestRates = new Dictionary<string, decimal>();
            foreach (var rate in rates.EnumerateObject())
            {
                latestRates[rate.Name] = rate.Value.GetDecimal();
            }

            return latestRates;
        }

    }
}