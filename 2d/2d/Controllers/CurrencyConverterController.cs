using _1c.Data;
using _2d.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _2d.Controllers
{
    public class CurrencyConverterController : Controller
    {
        private readonly ExchangeRateContext _context;
        public CurrencyConverterController(ExchangeRateContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            var model = new ExchangeRateViewModel
            {
                ExchangeRates = new List<ExchangeRateData>()
            };
            return View(model);
        }


        public async Task<IActionResult> IndexWithParameters(string currencyCode, DateTime startDate, DateTime endDate)
        {
            try
            {
                var rates = await _context.ExchangeRates
                .Where(rate => rate.ToCurrency == currencyCode &&
                   rate.Date.HasValue &&
                   rate.Date.Value.Date >= startDate.Date &&
                   rate.Date.Value.Date <= endDate.Date)
                .Select(rate => new ExchangeRateData
                {
                    Date = rate.Date.Value.Date, // Use the Date part only for comparison
                    Rate = rate.Rate
                })
                .ToListAsync();


                var exchangeRateViewModel = new ExchangeRateViewModel
                {
                    ExchangeRates = rates
                };
                return View("Index", exchangeRateViewModel);
            }
            catch (Exception ex)
            {

                var errorViewModel = new ErrorViewModel
                {
                    ErrorMessage = "An error occurred while processign your request."
                };
                return View("Error", errorViewModel);
            }

        }
    }
}
