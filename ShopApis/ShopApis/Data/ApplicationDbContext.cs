using Microsoft.EntityFrameworkCore;
using ShopApis.Models;

namespace ShopApis.Data
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> option) : base(option)
        {

        }


        //public DbSet<Category> Categories => Set<Category>();
        //public DbSet<Product> Products => Set<Product>();
        //public DbSet<Order> Orders => Set<Order>();

        public DbSet<Category> Categories { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<Order> Orders { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(e =>
            {
                e.Property(c => c.Name).IsRequired().HasMaxLength(100);
                e.Property(c => c.Description).HasMaxLength(500);
                e.HasIndex(c => c.Name).IsUnique();
            });


            modelBuilder.Entity<Product>(e =>
            {
                e.Property(p => p.Name).IsRequired().HasMaxLength(150);
                e.Property(p => p.Price).HasPrecision(18, 2);

                e.HasOne(p => p.Category)
                 .WithMany(c => c.Products)
                 .HasForeignKey(p => p.CategoryId)
                 .OnDelete(DeleteBehavior.Restrict);
            });


            modelBuilder.Entity<Order>(e =>
            {
                e.Property(o => o.CustomerName).IsRequired().HasMaxLength(100);
                e.Property(o => o.Status).HasMaxLength(30);
                e.Property(o => o.TotalAmount).HasPrecision(18, 2);

                e.HasOne(o => o.Product)
                 .WithMany(p => p.Orders)
                 .HasForeignKey(o => o.ProductId)
                 .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
