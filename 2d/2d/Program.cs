using _1c.Data;
using Microsoft.EntityFrameworkCore;

namespace _2d
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            //Add Dbcontext with SQL Server provider 
            builder.Services.AddDbContext<ExchangeRateContext>(options =>
                           options.UseSqlServer(builder.Configuration.GetConnectionString("ExchangeRateDb")));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=CurrencyConverter}/{action=Index}/{id?}");

            app.Run();
        }
    }
}