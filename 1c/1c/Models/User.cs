using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1c.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ExchangeRate> ExchangeRates { get; set; }
    }
}
