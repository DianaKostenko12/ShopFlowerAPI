using DAL.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;

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

            base.OnModelCreating(modelBuilder);
        }
    }
}
