/*
 b. Extend the program with an optional input date, do the same calculation as in step 1 but use 
the currency rate for the date inputted to the program. You need to find out the url for 
retrieving exchange rate a given date using the documentation on fixer.io.
 */

using System.Globalization;
using System.Text.Json;

namespace _1b
{
    internal class Program
    {
        private const string ApiKey = "Your Api key";
        private static readonly HttpClient _httpClient = new HttpClient();
        static async Task Main(string[] args)
        {
            Console.WriteLine("Enter your currency code:");
            string fromCurrency = Console.ReadLine().ToUpper();

            Console.WriteLine("Enter the currency code you want to convert to:");
            string toCurrency = Console.ReadLine().ToUpper();

            Console.WriteLine("Enter the amount you want to convert:");
            string amountInput = Console.ReadLine();
            if (!decimal.TryParse(amountInput, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal amount))
            {
                Console.WriteLine("Invalid amount");
                return;
            }

            Console.WriteLine("Enter the date for the conversin in (DD.MM.YYYY), or press Enter for the latest rate");
            string dateInput = Console.ReadLine();
            DateTime date;
            bool useHistoricalData = DateTime.TryParseExact(dateInput, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
            
            
            decimal convertedAmount = await ConvertCurrency(fromCurrency, toCurrency, amount, date, useHistoricalData);
            string dateString = useHistoricalData ? date.ToString("dd.MM.yyyy") : "today's";
            Console.WriteLine($"{amount.ToString(CultureInfo.InvariantCulture)} {fromCurrency} on {dateString} is {convertedAmount.ToString(CultureInfo.InvariantCulture)} {toCurrency}");
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        private static async Task<decimal> ConvertCurrency(string fromCurrency, string toCurrency, decimal amount, DateTime date, bool useHistoricalData)
        {
            string url = useHistoricalData ? $"http://data.fixer.io/api/{date:yyyy-MM-dd}?access_key={ApiKey}&symbols={fromCurrency},{toCurrency}"
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