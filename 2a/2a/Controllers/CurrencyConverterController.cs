using _2a.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace _2a.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyConverterController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private const string ApiKey = "Your API key";
        public CurrencyConverterController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Retrieves the latest exchange rates from fixer.io.
        /// gives the date in format yyyy-MM-dd (optional)
        /// </summary>

        [HttpGet()]
        public async Task<ActionResult> GetAllExchangeRates(DateTime? date = null)
        {
            try
            {
                string url;
                if (date.HasValue)
                {
                    // Historical rates endpoint
                    url = $"http://data.fixer.io/api/{date:yyyy-MM-dd}?access_key={ApiKey}";
                }
                else
                {
                    // Latest rates endpoint
                    url = $"http://data.fixer.io/api/latest?access_key={ApiKey}";
                }
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                var ratesData = JsonSerializer.Deserialize<dynamic>(responseBody);
                return Ok(ratesData);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while connecting to the exchange rates service.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing the exchange rates.");
            }
        }


        /// <summary>
        /// Converts an amount from one currency to another using current or historical exchange rates.
        /// 2a
        /// </summary>
        [HttpPost("convert")]
        public async Task<ActionResult<ConversionResposne>> ConvertCurrency([FromBody] ConversionRequest request)
        {
            try
            {
                var convertedAmount = await ConvertCurrency(request.FromCurrency, request.ToCurrency, request.Amount, request.Date);
                return new ConversionResposne { ConvertedAmount = convertedAmount };

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private async Task<decimal> ConvertCurrency(string fromCurrency, string toCurrency, decimal amount, DateTime? date)
        {
            string url = date.HasValue
                  ? $"http://data.fixer.io/api/{date:yyyy-MM-dd}?access_key={ApiKey}&symbols={fromCurrency},{toCurrency}"
                : $"http://data.fixer.io/api/latest?access_key={ApiKey}&symbols={fromCurrency},{toCurrency}";
            HttpResponseMessage response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            using JsonDocument document = JsonDocument.Parse(responseBody);
            JsonElement root = document.RootElement;
            JsonElement rates = root.GetProperty("rates");
            decimal fromRate = rates.GetProperty(fromCurrency).GetDecimal();
            decimal toRate = rates.GetProperty(toCurrency).GetDecimal();
            return amount * toRate / fromRate;
        }
    }
}
