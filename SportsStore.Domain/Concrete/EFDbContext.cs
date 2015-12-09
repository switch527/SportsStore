using SportsStore.Domain.Entities;
using System.Data.Entity;
namespace SportsStore.Domain.Concrete
{
    public class EFDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderLine> OrderLines { get; set; }
        public DbSet<CreditCard> CreditCards { get; set; }
    }
}
