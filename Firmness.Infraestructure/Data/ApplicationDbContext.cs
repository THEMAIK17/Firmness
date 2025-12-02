using Firmness.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Firmness.Infraestructure.Data;



public class ApplicationDbContext : IdentityDbContext<Client>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Register the entities created in the Domain layer
    public DbSet<Product> Products { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<SaleDetail> SaleDetails { get; set; }
        
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
            
        // Renames Identity tables for a cleaner structure 
        builder.HasDefaultSchema("Identity");
            
        // Ensures correct precision for monetary values in PostgreSQL
        builder.Entity<Product>()
            .Property(p => p.UnitPrice)
            .HasColumnType("decimal(18, 2)");
            
        builder.Entity<Sale>()
            .Property(s => s.TotalAmount)
            .HasColumnType("decimal(18, 2)");
                
        builder.Entity<Sale>()
            .Property(s => s.TaxAmount)
            .HasColumnType("decimal(18, 2)");
                
        builder.Entity<SaleDetail>()
            .Property(sd => sd.UnitPriceAtSale)
            .HasColumnType("decimal(18, 2)");
    }
}