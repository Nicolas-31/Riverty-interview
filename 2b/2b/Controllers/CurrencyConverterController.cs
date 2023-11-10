using _2b.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace _2b.Controllers
{
    public class CurrencyConverterController : Controller
    {
        private readonly HttpClient _httpClient;
        private const string ApiKey = "Your Api Key"; 
        private const string BaseUrl = "http://data.fixer.io/api/"; 

        public CurrencyConverterController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public IActionResult Index()
        {
            return View(new ConversionRequest());
        }

        [HttpPost]
        public async Task<IActionResult> ConvertCurrency(ConversionRequest request)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", request);
            }

            string url = request.Date.HasValue
                ? $"{BaseUrl}{request.Date:yyyy-MM-dd}?access_key={ApiKey}&symbols={request.FromCurrency},{request.ToCurrency}"
                : $"{BaseUrl}latest?access_key={ApiKey}&symbols={request.FromCurrency},{request.ToCurrency}";

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                using JsonDocument document = JsonDocument.Parse(responseBody);
                JsonElement root = document.RootElement;
                JsonElement rates = root.GetProperty("rates");

                decimal fromRate = rates.GetProperty(request.FromCurrency).GetDecimal();
                decimal toRate = rates.GetProperty(request.ToCurrency).GetDecimal();
                decimal convertedAmount = request.Amount * toRate / fromRate;

                var conversionResponse = new ConversionResponse { ConvertedAmount = convertedAmount };

                return View("ConversionResult", conversionResponse);
            }
            catch (HttpRequestException ex)
            {
                // Handle HTTP request exceptions
                return View("Error", new ErrorViewModel { ErrorMessage = "Error connecting to the API: " + ex.Message });
            }
            catch (Exception ex)
            {
                // Handle general exceptions
                return View("Error", new ErrorViewModel { ErrorMessage = "An error occurred: " + ex.Message });
            }
        }
    }
}
