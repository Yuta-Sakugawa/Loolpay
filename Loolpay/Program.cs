using Microsoft.EntityFrameworkCore;
using Loolpay.Data;
using Loolpay.Models;
using Microsoft.AspNetCore.Identity;

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
                options.UseSqlite(connectionString));

            builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

            builder.Services.AddControllersWithViews()
                .AddViewLocalization()
                .AddDataAnnotationsLocalization();
            builder.Services.AddRazorPages();

            var app = builder.Build();

            // Localization
            var supportedCultures = new[] { "ja", "en" };
            var localizationOptions = new RequestLocalizationOptions()
                .SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);

            app.UseRequestLocalization(localizationOptions);

            // Auto-migrate database
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<Loolpay.Data.ApplicationDbContext>();
                context.Database.Migrate();

                // Seed Stores
                if (!context.Stores.Any())
                {
                    context.Stores.AddRange(
                        new Store { StoreName = "セブンイレブン 渋谷店", StoreAddress = "東京都渋谷区...", Pay = "PayPay, クレジットカード" },
                        new Store { StoreName = "ローソン 新宿店", StoreAddress = "東京都新宿区...", Pay = "LINE Pay, 現金" }
                    );
                    context.SaveChanges();
                }
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication(); 
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }
    }
}
