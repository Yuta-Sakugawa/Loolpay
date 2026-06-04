using Microsoft.EntityFrameworkCore;
using Loolpay.Data;
using Loolpay.Models;
using Microsoft.AspNetCore.Identity;

namespace Loolpay
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<Loolpay.Data.ApplicationDbContext>(options =>
                options.UseSqlite(connectionString));

            builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
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

            // Auto-migrate database & Seed Admin
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<Loolpay.Data.ApplicationDbContext>();
                context.Database.Migrate();

                // Role/User Seeding
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                
                if (!await roleManager.RoleExistsAsync("admin"))
                {
                    await roleManager.CreateAsync(new IdentityRole("admin"));
                }
                
                var adminEmail = "admin@icloud.com";
                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                if (adminUser == null)
                {
                    adminUser = new ApplicationUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
                    await userManager.CreateAsync(adminUser, "Admin777!");
                    await userManager.AddToRoleAsync(adminUser, "admin");
                }
                else if (!await userManager.IsInRoleAsync(adminUser, "admin"))
                {
                    await userManager.AddToRoleAsync(adminUser, "admin");
                }

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

            await app.RunAsync();
        }
    }
}
