using Enhanzer.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Enhanzer.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<LocationDetail> LocationDetails => Set<LocationDetail>();

    public DbSet<PurchaseBill> PurchaseBills => Set<PurchaseBill>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<LocationDetail>(entity =>
        {
            entity.ToTable("Location_Details");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.LocationCode)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(x => x.LocationName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(x => x.CreatedAt)
                .IsRequired();

            entity.HasIndex(x => x.LocationCode)
                .IsUnique();
        });

        modelBuilder.Entity<PurchaseBill>(entity =>
        {
            entity.ToTable("PurchaseBills");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.ItemName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.BatchName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(x => x.StandardCost)
                .HasColumnType("decimal(18,2)");

            entity.Property(x => x.StandardPrice)
                .HasColumnType("decimal(18,2)");

            entity.Property(x => x.DiscountPercentage)
                .HasColumnType("decimal(5,2)");

            entity.Property(x => x.TotalCost)
                .HasColumnType("decimal(18,2)");

            entity.Property(x => x.TotalSelling)
                .HasColumnType("decimal(18,2)");
        });
    }
}