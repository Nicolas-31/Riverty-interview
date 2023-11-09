namespace _2a.Models
{
    public class ConversionRequest
    {
        private string _fromCurrency;
        private string _toCurrency;
        public string FromCurrency
        {
            get => _fromCurrency;
            set => _fromCurrency = value?.ToUpper();
        }
        public string ToCurrency
        {
            get => _toCurrency;
            set => _toCurrency = value?.ToUpper();
        }
        public decimal Amount { get; set; }
        public DateTime? Date { get; set; }
    }
}
