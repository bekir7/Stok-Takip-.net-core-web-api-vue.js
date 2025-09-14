using Microsoft.EntityFrameworkCore;
using stokTakip.Models;

namespace stokTakip.Data
{
    public class StokTakipDbContext : DbContext
    {
        public StokTakipDbContext(DbContextOptions<StokTakipDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<StockMovement> StockMovements { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleItem> SaleItems { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<PurchaseItem> PurchaseItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Product configurations
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.ProductId);
                entity.Property(e => e.ProductName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.HasIndex(e => e.ProductCode).IsUnique().HasFilter("[ProductCode] IS NOT NULL");
            });

            // StockMovement configurations
            modelBuilder.Entity<StockMovement>(entity =>
            {
                entity.HasKey(e => e.StockMovementId);
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.MovementDate).HasDefaultValueSql("GETUTCDATE()");
                entity.HasOne(e => e.Product)
                    .WithMany(p => p.StockMovements)
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Customer configurations
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.CustomerId);
                entity.Property(e => e.CustomerName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.CreditLimit).HasColumnType("decimal(18,2)").HasDefaultValue(0);
                entity.Property(e => e.CurrentBalance).HasColumnType("decimal(18,2)").HasDefaultValue(0);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
            });

            // Supplier configurations
            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.HasKey(e => e.SupplierId);
                entity.Property(e => e.SupplierName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.CreditLimit).HasColumnType("decimal(18,2)").HasDefaultValue(0);
                entity.Property(e => e.CurrentBalance).HasColumnType("decimal(18,2)").HasDefaultValue(0);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
            });

            // Sale configurations
            modelBuilder.Entity<Sale>(entity =>
            {
                entity.HasKey(e => e.SaleId);
                entity.Property(e => e.SaleNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.SubTotal).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TaxAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.SaleDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.HasOne(e => e.Customer)
                    .WithMany(c => c.Sales)
                    .HasForeignKey(e => e.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // SaleItem configurations
            modelBuilder.Entity<SaleItem>(entity =>
            {
                entity.HasKey(e => e.SaleItemId);
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18,2)").HasDefaultValue(0);
                entity.Property(e => e.TaxRate).HasColumnType("decimal(18,2)").HasDefaultValue(0);
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
                entity.HasOne(e => e.Sale)
                    .WithMany(s => s.SaleItems)
                    .HasForeignKey(e => e.SaleId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Product)
                    .WithMany(p => p.SaleItems)
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Purchase configurations
            modelBuilder.Entity<Purchase>(entity =>
            {
                entity.HasKey(e => e.PurchaseId);
                entity.Property(e => e.PurchaseNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.SubTotal).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TaxAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.PurchaseDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.HasOne(e => e.Supplier)
                    .WithMany(s => s.Purchases)
                    .HasForeignKey(e => e.SupplierId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // PurchaseItem configurations
            modelBuilder.Entity<PurchaseItem>(entity =>
            {
                entity.HasKey(e => e.PurchaseItemId);
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18,2)").HasDefaultValue(0);
                entity.Property(e => e.TaxRate).HasColumnType("decimal(18,2)").HasDefaultValue(0);
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
                entity.HasOne(e => e.Purchase)
                    .WithMany(p => p.PurchaseItems)
                    .HasForeignKey(e => e.PurchaseId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Product)
                    .WithMany(p => p.PurchaseItems)
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
