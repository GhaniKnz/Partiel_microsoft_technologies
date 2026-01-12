using ECommerceApi.Controllers;
using ECommerceApi.Models;
using ECommerceApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApi.Tests;

// Tests pour ProductsController
public class ProductsControllerTests
{
    [Fact]
    public void GetProducts_ShouldReturnListOfProducts()
    {
        // Arrange
        var stockService = new StockService();
        var controller = new ProductsController(stockService);

        // Act
        var result = controller.GetProducts();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var products = Assert.IsType<List<ProductDto>>(okResult.Value);
        Assert.Equal(5, products.Count);
    }

    [Fact]
    public void GetProducts_ShouldReturnCorrectFormat()
    {
        // Arrange
        var stockService = new StockService();
        var controller = new ProductsController(stockService);

        // Act
        var result = controller.GetProducts();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var products = Assert.IsType<List<ProductDto>>(okResult.Value);
        
        // Vérifier le format des produits
        foreach (var product in products)
        {
            Assert.True(product.Id > 0);
            Assert.False(string.IsNullOrEmpty(product.Name));
            Assert.True(product.Price > 0);
            Assert.True(product.Stock >= 0);
        }
    }
}
