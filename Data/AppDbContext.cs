using SupplyManagement.Models;
namespace SupplyManagement.Data;
using Microsoft.EntityFrameworkCore;
public class SupplyDbContext : DbContext
{
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Warehouse> Warehouses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=SupplyDB;Trusted_Connection=True;");
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasIndex(p => p.Name)
            .IsUnique();
            entity.Property(p => p.Price)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
               entity.ToTable(t=>t.HasCheckConstraint("CK_Product_Price", "Price>0"));
            entity.Property(p => p.StockQuantity)
            .IsRequired();
            entity.ToTable(t=>t.HasCheckConstraint("CK_Product_StockQuantity", "StockQuantity>=0"));
            entity.HasOne(p => p.Warehouse)
            .WithMany(w => w.Products)
            .HasForeignKey(p => p.WarehouseId)
            .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(p => p.Supplier)
            .WithMany(w => w.Products)
            .HasForeignKey(p => p.SupplierId)
            .OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<Supplier>(entity =>
        {

            entity.Property(s => s.Name)
                      .IsRequired()
                      .HasMaxLength(100);

            entity.Property(s => s.Contact)
                  .IsRequired()
                  .HasMaxLength(100);

        });
        modelBuilder.Entity<Transaction>(entity =>
            {

                entity.Property(t => t.TransactionType)
                          .IsRequired()
                          .HasMaxLength(3);
                entity.Property(t => t.Quantity)
          .IsRequired();
          entity.ToTable(t=>t.HasCheckConstraint("CK_Transaction_Quantity", "Quantity > 0"));

                entity.Property(t => t.Date)
                          .IsRequired();
                entity.HasOne(t=>t.Product)
                .WithMany(p=>p.Transactions).HasForeignKey(t => t.ProductId).OnDelete(DeleteBehavior.Cascade);

            });



    }
    
}