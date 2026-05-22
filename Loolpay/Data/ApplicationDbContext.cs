using Microsoft.EntityFrameworkCore;
using Loolpay.Models;

namespace Loolpay.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Store> Stores { get; set; }
    }
}
