using DAL.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DAL.Models.Orders;
using DAL.Models.Flowers;

namespace DAL.Data
{
    public class DataContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<Flower> Flowers { get; set; }
        public DbSet<Bouquet> Bouquets { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<BouquetFlower> BouquetFlowers { get; set; }
        public DbSet<OrderBouquet> OrderBouquets { get; set; }
        public DbSet<WrappingPaper> WrappingPapers { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BouquetFlower>()
                .HasKey(bf => new { bf.BouquetId, bf.FlowerId });
            modelBuilder.Entity<BouquetFlower>()
                .HasOne(b => b.Bouquet)
                .WithMany(bf => bf.BouquetsFlowers)
                .HasForeignKey(b => b.BouquetId);
            modelBuilder.Entity<BouquetFlower>()
                .HasOne(f => f.Flower)
                .WithMany(bf => bf.BouquetsFlowers)
                .HasForeignKey(f => f.FlowerId);

            modelBuilder.Entity<BouquetFlower>()
                .Property(bf => bf.Role)
                .HasConversion<string>();

            modelBuilder.Entity<OrderBouquet>()
                .HasKey(ob => new { ob.OrderId, ob.BouquetId });
            modelBuilder.Entity<OrderBouquet>()
                .HasOne(b => b.Bouquet)
                .WithMany(ob => ob.OrderBouquets)
                .HasForeignKey(b => b.BouquetId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<OrderBouquet>()
                .HasOne(o => o.Order)
                .WithMany(ob => ob.OrderBouquets)
                .HasForeignKey(o => o.OrderId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Color>(entity =>
            {
                entity.HasKey(e => e.ColorId);
                entity.Property(e => e.ColorName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Shade).HasMaxLength(50);
                entity.HasIndex(e => e.ColorName).IsUnique();
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.CategoryId);
                entity.Property(e => e.CategoryName).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.CategoryName).IsUnique();
            });

            modelBuilder.Entity<WrappingPaper>(entity =>
            {
                entity.HasKey(e => e.WrappingPaperId);
                entity.Property(e => e.Type).HasConversion<int>();
                entity.Property(e => e.Pattern).HasConversion<int>();
                entity.Property(e => e.IsAvailable).HasDefaultValue(true);

                entity.HasOne(e => e.Color)
                      .WithMany()
                      .HasForeignKey(e => e.ColorId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Flower>(entity =>
            {
                entity.Property(e => e.FlowerCost)
                      .HasColumnType("decimal(18,2)");

                entity.HasOne(e => e.Color)
                      .WithMany(c => c.Flowers)
                      .HasForeignKey(e => e.ColorId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Category)
                      .WithMany(c => c.Flowers)
                      .HasForeignKey(e => e.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
