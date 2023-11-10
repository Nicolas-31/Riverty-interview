namespace _1c.Models
{
    public class ExchangeRate
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public string FromCurrency { get; set; }
        public string ToCurrency { get; set; }
        public decimal Rate { get; set; }
        public User User { get; set; }
    }
}
