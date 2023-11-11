namespace _2d.Models
{
    public class ExchangeRateViewModel
    {
        public List<ExchangeRateData> ExchangeRates { get; set; }
    }

    public class ExchangeRateData
    {
        public DateTime Date { get; set; }
        public decimal Rate { get; set; }
    }
}
