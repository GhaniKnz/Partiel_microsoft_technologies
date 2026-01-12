using ECommerceApi.Data;
using ECommerceApi.Models;

namespace ECommerceApi.Services;

// Service de stock utilisant EF Core (base de données in-memory)
// Implémente la même interface que le singleton StockService
public class DatabaseStockService : IStockService
{
    private readonly ECommerceDbContext _context;

    public DatabaseStockService(ECommerceDbContext context)
    {
        _context = context;
    }

    public List<Product> GetAllProducts()
    {
        return _context.Products
            .Select(p => new Product
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Stock = p.Stock
            })
            .ToList();
    }

    public Product? GetProductById(int id)
    {
        var entity = _context.Products.FirstOrDefault(p => p.Id == id);
        if (entity == null) return null;

        return new Product
        {
            Id = entity.Id,
            Name = entity.Name,
            Price = entity.Price,
            Stock = entity.Stock
        };
    }

    public bool UpdateStock(int productId, int quantityToReserve)
    {
        var product = _context.Products.FirstOrDefault(p => p.Id == productId);
        if (product == null || product.Stock < quantityToReserve)
            return false;

        product.Stock -= quantityToReserve;
        _context.SaveChanges();
        return true;
    }

    public bool IsStockSufficient(int productId, int quantity)
    {
        var product = _context.Products.FirstOrDefault(p => p.Id == productId);
        return product != null && product.Stock >= quantity;
    }
}
