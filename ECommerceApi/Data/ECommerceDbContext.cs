using Microsoft.EntityFrameworkCore;

namespace ECommerceApi.Data;

// DbContext pour l'API e-commerce avec EF Core
public class ECommerceDbContext : DbContext
{
    public ECommerceDbContext(DbContextOptions<ECommerceDbContext> options) : base(options)
    {
    }

    // Table des produits
    public DbSet<ProductEntity> Products { get; set; }

    // Table des codes promos
    public DbSet<PromoCodeEntity> PromoCodes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Données initiales pour les produits
        modelBuilder.Entity<ProductEntity>().HasData(
            new ProductEntity { Id = 1, Name = "Ordinateur Portable", Price = 899.99m, Stock = 15 },
            new ProductEntity { Id = 2, Name = "Souris Gaming", Price = 49.99m, Stock = 50 },
            new ProductEntity { Id = 3, Name = "Clavier Mécanique", Price = 129.99m, Stock = 30 },
            new ProductEntity { Id = 4, Name = "Écran 27 pouces", Price = 349.99m, Stock = 10 },
            new ProductEntity { Id = 5, Name = "Casque Audio", Price = 79.99m, Stock = 25 }
        );

        // Données initiales pour les codes promos
        modelBuilder.Entity<PromoCodeEntity>().HasData(
            new PromoCodeEntity { Id = 1, Code = "DISCOUNT20", DiscountPercent = 20m },
            new PromoCodeEntity { Id = 2, Code = "DISCOUNT10", DiscountPercent = 10m }
        );
    }
}
