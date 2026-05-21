using Microsoft.EntityFrameworkCore;
using Loolpay.Data;

namespace Loolpay
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<Loolpay.Data.ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Auto-migrate database
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<Loolpay.Data.ApplicationDbContext>();
                context.Database.Migrate();

                // Seed Users
                if (!context.Users.Any())
                {
                    context.Users.AddRange(
                        new Loolpay.Models.User { Name = "Alice", Email = "alice@example.com" },
                        new Loolpay.Models.User { Name = "Bob", Email = "bob@example.com" },
                        new Loolpay.Models.User { Name = "田中太郎", Email = "aaa@gmail.com" }
                    );
                }

                // Seed Products
                if (!context.Products.Any())
                {
                    context.Products.AddRange(
                        new Loolpay.Models.Product { Name = "Laptop", Price = 1200.00m, Stock = 10 },
                        new Loolpay.Models.Product { Name = "Mouse", Price = 25.50m, Stock = 50 }
                    );
                }
                context.SaveChanges();
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
