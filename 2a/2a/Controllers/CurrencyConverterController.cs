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
        private const string ApiKey = "Your Api Key";
        public CurrencyConverterController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Converts an amount from one currency to another using current or historical exchange rates.
        /// </summary>
        /// <param name="request">A <see cref="ConversionRequest"/> object containing the details of the conversion: 
        /// from currency, to currency, the amount to convert, and an optional date for historical rates.</param>
        /// <returns>
        /// An ActionResult of type <see cref="ConversionResponse"/> that contains the converted amount if the conversion is successful;
        /// otherwise, returns a BadRequest with the error message.
        /// </returns>
        /// <remarks>
        /// This endpoint receives a POST request with the conversion details. 
        /// It performs an asynchronous operation to convert the specified amount from the source to the target currency.
        /// If the conversion is successful, a <see cref="ConversionResponse"/> is returned with the result.
        /// In case of an error, such as an invalid currency code or an exception in the conversion process,
        /// a BadRequest is returned along with the error details.
        /// </remarks>
        [HttpPost("convert")]
        public async Task<ActionResult<ConversionResposne>> ConvertCurrency([FromBody] ConversionRequest request)
        {
            try
            {
                var convertedAmount = await ConvertCurrency(request.FromCurrency, request.ToCurrency, request.Amount, request.Date);
                return new ConversionResposne { ConvertedAmount = convertedAmount };

            }
            catch(Exception ex)
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
