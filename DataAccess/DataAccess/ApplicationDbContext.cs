using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace SalesWPFApp
{
    public class AppDbContext : DbContext
    {
        private readonly IConfiguration _configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Member> Members { get; set; }
        public dynamic account { get; set; }
        public Member member { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_configuration["ConnectionStrings:DefaultConnection"]);
        }
    }
}
