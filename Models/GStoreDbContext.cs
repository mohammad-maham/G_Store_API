using Microsoft.EntityFrameworkCore;

namespace GoldStore.Models;

public partial class GStoreDbContext : DbContext
{
    private readonly IConfiguration _config;
    public GStoreDbContext()
    {
        _config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
    }

    public GStoreDbContext(DbContextOptions<GStoreDbContext> options)
        : base(options)
    {
        _config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
    }

    public virtual DbSet<Amount> Amounts { get; set; }

    public virtual DbSet<AmountThreshold> AmountThresholds { get; set; }

    public virtual DbSet<ArchiveAmountThreshold> ArchiveAmountThresholds { get; set; }

    public virtual DbSet<ArchiveGoldRepository> ArchiveGoldRepositories { get; set; }

    public virtual DbSet<ArchiveRepository> ArchiveRepositories { get; set; }

    public virtual DbSet<AvailabilityType> AvailabilityTypes { get; set; }

    public virtual DbSet<DollarPrice> DollarPrices { get; set; }

    public virtual DbSet<Entity> Entities { get; set; }

    public virtual DbSet<EntityMode> EntityModes { get; set; }

    public virtual DbSet<EntityType> EntityTypes { get; set; }

    public virtual DbSet<GoldEntity> GoldEntities { get; set; }

    public virtual DbSet<GoldMaintenanceType> GoldMaintenanceTypes { get; set; }

    public virtual DbSet<GoldPrice> GoldPrices { get; set; }

    public virtual DbSet<GoldRepository> GoldRepositories { get; set; }

    public virtual DbSet<GoldRepositoryTransaction> GoldRepositoryTransactions { get; set; }

    public virtual DbSet<GoldType> GoldTypes { get; set; }

    public virtual DbSet<MaintenanceType> MaintenanceTypes { get; set; }

    public virtual DbSet<Material> Materials { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductRepository> ProductRepositories { get; set; }

    public virtual DbSet<ProductRepositoryTransaction> ProductRepositoryTransactions { get; set; }

    public virtual DbSet<ProductType> ProductTypes { get; set; }

    public virtual DbSet<ProductUserEffect> ProductUserEffects { get; set; }

    public virtual DbSet<Repository> Repositories { get; set; }

    public virtual DbSet<RepositoryTransaction> RepositoryTransactions { get; set; }

    public virtual DbSet<SilverPrice> SilverPrices { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<TeterPrice> TeterPrices { get; set; }

    public virtual DbSet<TransactionType> TransactionTypes { get; set; }

    public virtual DbSet<Unit> Units { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_config.GetConnectionString("GStoreDbContext"), x => x.UseNodaTime());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Amount>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Amount_pkey");

            entity.ToTable("Amount");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(100);
        });

        modelBuilder.Entity<AmountThreshold>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("AmountThreshold_pkey");

            entity.ToTable("AmountThreshold");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasIdentityOptions(null, null, 1000000000L, 1000000000000000000L, null, null);
            entity.Property(e => e.AmountId).HasDefaultValue(0);
        });

        modelBuilder.Entity<ArchiveAmountThreshold>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ArchiveAmountThreshold");

            entity.Property(e => e.AmountId).HasDefaultValue(0);
        });

        modelBuilder.Entity<ArchiveGoldRepository>(entity =>
        {
            entity.HasKey(e => e.ArchiveId).HasName("ArchiveGoldRepository_pkey");

            entity.ToTable("ArchiveGoldRepository");

            entity.Property(e => e.ArchiveId)
                .UseIdentityAlwaysColumn()
                .HasIdentityOptions(null, null, 1000000000L, 1000000000000000000L, null, null);
            entity.Property(e => e.ArchiveOperation).HasColumnType("character varying");
            entity.Property(e => e.CaratologyInfo).HasColumnType("json");
            entity.Property(e => e.GoldMaintenanceType).HasDefaultValue((short)1);
        });

        modelBuilder.Entity<ArchiveRepository>(entity =>
        {
            entity.HasKey(e => e.ArchiveId).HasName("ArchiveRepository_pkey");

            entity.ToTable("ArchiveRepository");

            entity.Property(e => e.ArchiveId)
                .UseIdentityAlwaysColumn()
                .HasIdentityOptions(null, null, 1000000000L, 1000000000000000000L, null, null);
            entity.Property(e => e.ArchiveOperation).HasColumnType("character varying");
            entity.Property(e => e.CaratologyInfo).HasColumnType("json");
            entity.Property(e => e.MaintenanceType).HasDefaultValue(1);
        });

        modelBuilder.Entity<AvailabilityType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("AvailabilityType_pkey");

            entity.ToTable("AvailabilityType");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(100);
        });

        modelBuilder.Entity<DollarPrice>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("DollarPrice_pkey");

            entity.ToTable("DollarPrice", "history");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Other).HasColumnType("json");
            entity.Property(e => e.Timestamp).HasDefaultValue(0L);
        });

        modelBuilder.Entity<Entity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Entity_pkey");

            entity.ToTable("Entity");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.AmountCode).HasDefaultValue(0);
            entity.Property(e => e.Caption).HasColumnType("character varying");
            entity.Property(e => e.EntityMode).HasDefaultValue(0);
            entity.Property(e => e.MaterialId).HasDefaultValue(0);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Scale).HasDefaultValueSql("1");
            entity.Property(e => e.Symbol).HasMaxLength(10);
        });

        modelBuilder.Entity<EntityMode>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("EntityModey_pkey");

            entity.ToTable("EntityMode");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Caption).HasColumnType("character varying");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<EntityType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("EntityType_pkey");

            entity.ToTable("EntityType");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Caption).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<GoldEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("GoldEntity_pkey");

            entity.ToTable("GoldEntity");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Caption).HasColumnType("character varying");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<GoldMaintenanceType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("GoldMaintenanceType_pkey");

            entity.ToTable("GoldMaintenanceType");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<GoldPrice>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("GoldPrice_pkey");

            entity.ToTable("GoldPrice", "history");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Other).HasColumnType("json");
            entity.Property(e => e.Timestamp).HasDefaultValue(0L);
        });

        modelBuilder.Entity<GoldRepository>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("GoldRepository_pkey");

            entity.ToTable("GoldRepository");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CaratologyInfo).HasColumnType("json");
            entity.Property(e => e.GoldMaintenanceType).HasDefaultValue((short)1);
            entity.Property(e => e.TransactionId).HasDefaultValue(0L);
        });

        modelBuilder.Entity<GoldRepositoryTransaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("GoldRepositoryTransactions_pkey");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.UserAdditionalData).HasColumnType("json");
            entity.Property(e => e.WalletInfo).HasColumnType("json");
        });

        modelBuilder.Entity<GoldType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("GoldType_pkey");

            entity.ToTable("GoldType");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<MaintenanceType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("MaintenanceType_pkey");

            entity.ToTable("MaintenanceType");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(100);
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
            entity.Property(e => e.Images).HasColumnType("json");
            entity.Property(e => e.Name).HasMaxLength(200);
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

        modelBuilder.Entity<ProductRepositoryTransaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ProductRepositoryTransaction_pkey");

            entity.ToTable("ProductRepositoryTransaction");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.DeliveryInfo).HasColumnType("json");
        });

        modelBuilder.Entity<ProductType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ProductType_pkey");

            entity.ToTable("ProductType");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.ProductTypeDefaultInfo).HasColumnType("json");
        });

        modelBuilder.Entity<ProductUserEffect>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ProductUserEffect_pkey");

            entity.ToTable("ProductUserEffect");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.LikeStatus).HasDefaultValue((short)0);
            entity.Property(e => e.Message).HasMaxLength(500);
            entity.Property(e => e.MessageDetail).HasColumnType("json");
        });

        modelBuilder.Entity<Repository>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Repository_pkey");

            entity.ToTable("Repository");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.AvailabilityInfo).HasColumnType("json");
            entity.Property(e => e.TransactionId).HasDefaultValue(0L);
        });

        modelBuilder.Entity<RepositoryTransaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("RepositoryTransactions_pkey");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.UserAdditionalData).HasColumnType("json");
            entity.Property(e => e.WalletInfo).HasColumnType("json");
        });

        modelBuilder.Entity<SilverPrice>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SilverPrice_pkey");

            entity.ToTable("SilverPrice", "history");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Other).HasColumnType("json");
            entity.Property(e => e.Timestamp).HasDefaultValue(0L);
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

        modelBuilder.Entity<TeterPrice>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("TeterPrice_pkey");

            entity.ToTable("TeterPrice", "history");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Other).HasColumnType("json");
            entity.Property(e => e.Timestamp).HasDefaultValue(0L);
        });

        modelBuilder.Entity<TransactionType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("TransactionType_pkey");

            entity.ToTable("TransactionType");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(100);
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
