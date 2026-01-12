using ECommerceApi.Controllers;
using ECommerceApi.Models;
using ECommerceApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApi.Tests;

// Tests pour OrdersController
public class OrdersControllerTests
{
    private static OrdersController CreateController()
    {
        var stockService = new StockService();
        var orderService = new OrderService(stockService);
        return new OrdersController(orderService);
    }

    [Fact]
    public void CreateOrder_ValidOrder_ShouldReturnOk()
    {
        // Arrange
        var controller = CreateController();
        var request = new OrderRequest
        {
            Products = new List<OrderProductRequest>
            {
                new() { Id = 2, Quantity = 2 }
            }
        };

        // Act
        var result = controller.CreateOrder(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<OrderResponse>(okResult.Value);
        Assert.NotNull(response.Products);
        Assert.Single(response.Products);
    }

    [Fact]
    public void CreateOrder_EmptyProducts_ShouldReturnBadRequest()
    {
        // Arrange
        var controller = CreateController();
        var request = new OrderRequest
        {
            Products = new List<OrderProductRequest>()
        };

        // Act
        var result = controller.CreateOrder(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);
        Assert.Contains("La commande doit contenir au moins un produit", errorResponse.Errors);
    }

    [Fact]
    public void CreateOrder_NullRequest_ShouldReturnBadRequest()
    {
        // Arrange
        var controller = CreateController();

        // Act
        var result = controller.CreateOrder(null!);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);
        Assert.Contains("La commande doit contenir au moins un produit", errorResponse.Errors);
    }

    [Fact]
    public void CreateOrder_NonExistingProduct_ShouldReturnBadRequest()
    {
        // Arrange
        var controller = CreateController();
        var request = new OrderRequest
        {
            Products = new List<OrderProductRequest>
            {
                new() { Id = 9999, Quantity = 1 }
            }
        };

        // Act
        var result = controller.CreateOrder(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);
        Assert.Contains("Le produit avec l'identifiant 9999 n'existe pas", errorResponse.Errors);
    }

    [Fact]
    public void CreateOrder_InsufficientStock_ShouldReturnBadRequest()
    {
        // Arrange
        var controller = CreateController();
        var request = new OrderRequest
        {
            Products = new List<OrderProductRequest>
            {
                new() { Id = 4, Quantity = 1000 } // Écran: stock = 10
            }
        };

        // Act
        var result = controller.CreateOrder(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);
        Assert.Single(errorResponse.Errors);
        Assert.Contains("exemplaire(s)", errorResponse.Errors[0]);
    }

    [Fact]
    public void CreateOrder_InvalidPromoCode_ShouldReturnBadRequest()
    {
        // Arrange
        var controller = CreateController();
        var request = new OrderRequest
        {
            Products = new List<OrderProductRequest>
            {
                new() { Id = 5, Quantity = 1 } // Casque: 79.99€ (> 50€)
            },
            PromoCode = "INVALID"
        };

        // Act
        var result = controller.CreateOrder(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);
        Assert.Contains("Le code promo est invalide", errorResponse.Errors);
    }

    [Fact]
    public void CreateOrder_PromoCodeUnder50_ShouldReturnBadRequest()
    {
        // Arrange
        var controller = CreateController();
        var request = new OrderRequest
        {
            Products = new List<OrderProductRequest>
            {
                new() { Id = 2, Quantity = 1 } // Souris: 49.99€ (< 50€)
            },
            PromoCode = "DISCOUNT20"
        };

        // Act
        var result = controller.CreateOrder(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);
        Assert.Contains("Les codes promos ne sont valables qu'à partir de 50 euros d'achat", errorResponse.Errors);
    }

    [Fact]
    public void CreateOrder_WithValidPromo_ShouldApplyDiscount()
    {
        // Arrange
        var controller = CreateController();
        var request = new OrderRequest
        {
            Products = new List<OrderProductRequest>
            {
                new() { Id = 5, Quantity = 1 } // Casque: 79.99€
            },
            PromoCode = "DISCOUNT20"
        };

        // Act
        var result = controller.CreateOrder(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<OrderResponse>(okResult.Value);
        Assert.Single(response.Discounts);
        Assert.Equal("promo", response.Discounts[0].Type);
    }

    [Fact]
    public void CreateOrder_WithMultipleErrors_ShouldReturnAllErrors()
    {
        // Arrange
        var controller = CreateController();
        var request = new OrderRequest
        {
            Products = new List<OrderProductRequest>
            {
                new() { Id = 9999, Quantity = 1 }, // Inexistant
                new() { Id = 8888, Quantity = 1 }  // Inexistant
            }
        };

        // Act
        var result = controller.CreateOrder(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);
        Assert.Equal(2, errorResponse.Errors.Count);
    }
}
