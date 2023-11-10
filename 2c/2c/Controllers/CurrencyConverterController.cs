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

        [HttpGet("rates/{currencyCode}/{startDate}/{endDate}")]
        public async Task<ActionResult<IEnumerable<ExchangeRate>>> GetRatesForPeriod(string currencyCode, DateTime startDate, DateTime endDate)
        {
            // Adjust the endDate to the end of the day to ensure all records are included
            endDate = endDate.Date.AddDays(1).AddTicks(-1);

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
