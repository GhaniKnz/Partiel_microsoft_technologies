using ECommerceApi.Models;

namespace ECommerceApi.Services;

// Interface pour le service de gestion du stock
public interface IStockService
{
    List<Product> GetAllProducts();
    Product? GetProductById(int id);
    bool UpdateStock(int productId, int quantityToReserve);
    bool IsStockSufficient(int productId, int quantity);
}

// Service singleton pour gérer le stock en mémoire
public class StockService : IStockService
{
    private readonly List<Product> _products;
    private readonly object _lock = new();

    public StockService()
    {
        // Initialisation avec 5 produits par défaut
        _products = new List<Product>
        {
            new Product { Id = 1, Name = "Ordinateur Portable", Price = 899.99m, Stock = 15 },
            new Product { Id = 2, Name = "Souris Gaming", Price = 49.99m, Stock = 50 },
            new Product { Id = 3, Name = "Clavier Mécanique", Price = 129.99m, Stock = 30 },
            new Product { Id = 4, Name = "Écran 27 pouces", Price = 349.99m, Stock = 10 },
            new Product { Id = 5, Name = "Casque Audio", Price = 79.99m, Stock = 25 }
        };
    }

    public List<Product> GetAllProducts()
    {
        lock (_lock)
        {
            // Retourne une copie pour éviter les modifications externes
            return _products.Select(p => new Product
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Stock = p.Stock
            }).ToList();
        }
    }

    public Product? GetProductById(int id)
    {
        lock (_lock)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product == null) return null;

            // Retourne une copie
            return new Product
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Stock = product.Stock
            };
        }
    }

    public bool UpdateStock(int productId, int quantityToReserve)
    {
        lock (_lock)
        {
            var product = _products.FirstOrDefault(p => p.Id == productId);
            if (product == null || product.Stock < quantityToReserve)
                return false;

            product.Stock -= quantityToReserve;
            return true;
        }
    }

    public bool IsStockSufficient(int productId, int quantity)
    {
        lock (_lock)
        {
            var product = _products.FirstOrDefault(p => p.Id == productId);
            return product != null && product.Stock >= quantity;
        }
    }
}
