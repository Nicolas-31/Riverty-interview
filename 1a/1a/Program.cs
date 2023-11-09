/*
Assignment 1a 
Create a console application that take two currency codes and one amount as input. The 
amount is in the first currency. The program should calculate the currency amount for the 
second currency code (using the latest exchange rates). The program should do the 
calculation in process and not utilize any external calculation api.
 */

using System.Globalization;
using System.Text.Json;

internal class Progam
{
    private const string ApiKey = "abd0a86748e44f2095d949529393812a";
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
        decimal convertedAmount = await ConvertCurrency(fromCurrency, toCurrency, amount);
        Console.WriteLine($"{amount.ToString(CultureInfo.InvariantCulture)} {fromCurrency} is {convertedAmount.ToString(CultureInfo.InvariantCulture)} {toCurrency}");
        Console.WriteLine("Press any key to exit");
        Console.ReadKey();
    }

    private static async Task<decimal> ConvertCurrency(string fromCurrency, string toCurrency, decimal amount)
    {
        string url = $"http://data.fixer.io/api/latest?access_key={ApiKey}&symbols={fromCurrency},{toCurrency}";
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
