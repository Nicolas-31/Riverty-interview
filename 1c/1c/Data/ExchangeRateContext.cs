using _1c.Models;
using Microsoft.EntityFrameworkCore;

namespace _1c.Data
{
    public class ExchangeRateContext: DbContext
    {
        public DbSet<ExchangeRate> ExchangeRates { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=localhost;Database=ExchangeRateDb;Trusted_Connection=True;TrustServerCertificate=true;");

        }
    }
    }

