using _1c.Models;
using Microsoft.EntityFrameworkCore;

namespace _1c.Data
{
    public class ExchangeRateContext: DbContext
    {
        public DbSet<ExchangeRate> ExchangeRates { get; set; }
        public DbSet<User> Users { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=your_server;Database=your_database;Trusted_Connection=True;");
        }
    }
}
