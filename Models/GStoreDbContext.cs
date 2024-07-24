using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GoldStore.Models;

public partial class GStoreDbContext : DbContext
{
    public GStoreDbContext()
    {
    }

    public GStoreDbContext(DbContextOptions<GStoreDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<GoldEntity> GoldEntities { get; set; }

    public virtual DbSet<GoldRepository> GoldRepositories { get; set; }

    public virtual DbSet<GoldRepositoryTransaction> GoldRepositoryTransactions { get; set; }

    public virtual DbSet<GoldType> GoldTypes { get; set; }

    public virtual DbSet<Material> Materials { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductRepository> ProductRepositories { get; set; }

    public virtual DbSet<ProductType> ProductTypes { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<Unit> Units { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=194.60.231.81:5432;Database=G_Store_DB;Username=postgres;Password=Maham@7796",
            x => x.UseNodaTime());
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GoldEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("GoldEntity_pkey");

            entity.ToTable("GoldEntity");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Caption).HasColumnType("character varying");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<GoldRepository>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("GoldRepository_pkey");

            entity.ToTable("GoldRepository");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CaratologyInfo).HasColumnType("json");
        });

        modelBuilder.Entity<GoldRepositoryTransaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("GoldRepositoryTransactions_pkey");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.WalletInfo).HasColumnType("json");
        });

        modelBuilder.Entity<GoldType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("GoldType_pkey");

            entity.ToTable("GoldType");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Material>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Material_pkey");

            entity.ToTable("Material");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Product_pkey");

            entity.ToTable("Product");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.DefaultWeight).HasDefaultValue(0);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.ProductInfo).HasColumnType("json");
        });

        modelBuilder.Entity<ProductRepository>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ProductRepository_pkey");

            entity.ToTable("ProductRepository");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.ProductCode)
                .HasDefaultValueSql("0")
                .HasColumnType("character varying");
            entity.Property(e => e.ProductCustomInfo).HasColumnType("json");
            entity.Property(e => e.Weight).HasDefaultValue(0);
        });

        modelBuilder.Entity<ProductType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ProductType_pkey");

            entity.ToTable("ProductType");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.ProductTypeDefaultInfo).HasColumnType("json");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Status_pkey");

            entity.ToTable("Status");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Caption).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Supplier_pkey");

            entity.ToTable("Supplier");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name).HasColumnType("character varying");
            entity.Property(e => e.SupplierInfo).HasColumnType("json");
        });

        modelBuilder.Entity<Unit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Unit_pkey");

            entity.ToTable("Unit");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
