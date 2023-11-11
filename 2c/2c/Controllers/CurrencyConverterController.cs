using _1c.Data;
using _1c.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _2c.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyConverterController : ControllerBase
    {
        private readonly ExchangeRateContext _context;
        public CurrencyConverterController(ExchangeRateContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get the exchange rate for a given currency code and period
        /// both startDate and endDate must be provided in YYYY-MM-DD format
        /// </summary>
        /// <param name="currencyCode"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>

        [HttpGet("rates/{currencyCode}/{startDate}/{endDate}")]
        public async Task<ActionResult<IEnumerable<ExchangeRate>>> GetRatesForPeriod(string currencyCode, DateTime startDate, DateTime endDate)
        {
            // Adjust the endDate to include the entire day, up until 23:59:59.999
            endDate = endDate.Date.AddDays(1).AddMilliseconds(-1);

            var rates = await _context.ExchangeRates
                                      .Where(rate => rate.ToCurrency == currencyCode &&
                                                     rate.Date >= startDate &&
                                                     rate.Date <= endDate)
                                      .ToListAsync();

            if (!rates.Any())
            {
                return NotFound($"No exchange rate data found for {currencyCode} in the specified period.");
            }

            return Ok(rates);
        }

    }
}
