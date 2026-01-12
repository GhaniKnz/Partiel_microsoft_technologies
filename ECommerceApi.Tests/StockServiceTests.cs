using ECommerceApi.Services;

namespace ECommerceApi.Tests;

// Tests pour le service StockService
public class StockServiceTests
{
    [Fact]
    public void GetAllProducts_ShouldReturn5Products()
    {
        // Arrange
        var stockService = new StockService();

        // Act
        var products = stockService.GetAllProducts();

        // Assert
        Assert.Equal(5, products.Count);
    }

    [Fact]
    public void GetProductById_ExistingProduct_ShouldReturnProduct()
    {
        // Arrange
        var stockService = new StockService();

        // Act
        var product = stockService.GetProductById(1);

        // Assert
        Assert.NotNull(product);
        Assert.Equal(1, product.Id);
        Assert.Equal("Ordinateur Portable", product.Name);
    }

    [Fact]
    public void GetProductById_NonExistingProduct_ShouldReturnNull()
    {
        // Arrange
        var stockService = new StockService();

        // Act
        var product = stockService.GetProductById(999);

        // Assert
        Assert.Null(product);
    }

    [Fact]
    public void UpdateStock_SufficientStock_ShouldDecreaseStock()
    {
        // Arrange
        var stockService = new StockService();
        var initialProduct = stockService.GetProductById(1);
        var initialStock = initialProduct!.Stock;

        // Act
        var result = stockService.UpdateStock(1, 5);
        var updatedProduct = stockService.GetProductById(1);

        // Assert
        Assert.True(result);
        Assert.Equal(initialStock - 5, updatedProduct!.Stock);
    }

    [Fact]
    public void UpdateStock_InsufficientStock_ShouldReturnFalse()
    {
        // Arrange
        var stockService = new StockService();
        var product = stockService.GetProductById(1);

        // Act
        var result = stockService.UpdateStock(1, product!.Stock + 100);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsStockSufficient_SufficientStock_ShouldReturnTrue()
    {
        // Arrange
        var stockService = new StockService();

        // Act
        var result = stockService.IsStockSufficient(1, 5);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsStockSufficient_InsufficientStock_ShouldReturnFalse()
    {
        // Arrange
        var stockService = new StockService();

        // Act
        var result = stockService.IsStockSufficient(1, 1000);

        // Assert
        Assert.False(result);
    }
}
