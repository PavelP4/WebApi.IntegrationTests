using Microsoft.EntityFrameworkCore;
using WebApi.IntegrationTests.Infrastructure.Entities;

namespace WebApi.IntegrationTests.Infrastructure
{
    public class AppDbContext: DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Store> Stores { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>().HasKey(x => x.Id);
            modelBuilder.Entity<Order>().Property(x => x.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Order>()
                .Property(x => x.Number).IsRequired().HasMaxLength(30);
            modelBuilder.Entity<Order>()
                .Property(x => x.Customer).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Order>()
                .HasOne(x => x.Store)
                .WithMany(x => x.Orders).HasForeignKey(x => x.StoreId).IsRequired();
            modelBuilder.Entity<Order>()
                .HasMany(x => x.OrderItems)
                .WithOne(x => x.Order).HasForeignKey(x => x.OrderId).IsRequired();

            modelBuilder.Entity<OrderItem>().HasKey(x => x.Id);
            modelBuilder.Entity<OrderItem>().Property(x => x.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<OrderItem>()
                    .Property(x => x.Name).IsRequired().HasMaxLength(50);

            modelBuilder.Entity<Store>().HasKey(x => x.Id);
            modelBuilder.Entity<Store>().Property(x => x.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Store>()
                .Property(x => x.Name).IsRequired().HasMaxLength(50);

            Seed(modelBuilder);
        }

        private void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Store>()
                .HasData(
                    new Store { Id = Constants.Db.Store.StoreAId, Name = "Store_A" },
                    new Store { Id = Constants.Db.Store.StoreBId, Name = "Store_B" },
                    new Store { Id = Constants.Db.Store.StoreCId, Name = "Store_C" }
                );
        }
    }
}
