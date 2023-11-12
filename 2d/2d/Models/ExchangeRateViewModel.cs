namespace _2d.Models
{
    public class ExchangeRateViewModel
    {
        public List<ExchangeRateData> ExchangeRates { get; set; }
    }

    public class ExchangeRateData
    {
        public string FromCurrency { get; set; }
        public string ToCurrency { get; set; }
        public DateTime Date { get; set; }
        public decimal Rate { get; set; }
        
    }
}
