using ECommerceApi.Models;
using ECommerceApi.Services;

namespace ECommerceApi.Tests;

// Tests pour le service OrderService
public class OrderServiceTests
{
    private IStockService CreateStockService() => new StockService();

    #region Tests de calcul du montant (Règle a)

    [Fact]
    public void ProcessOrder_SimpleOrder_ShouldCalculateTotalCorrectly()
    {
        // Arrange
        var stockService = CreateStockService();
        var orderService = new OrderService(stockService);
        var request = new OrderRequest
        {
            Products = new List<OrderProductRequest>
            {
                new() { Id = 2, Quantity = 2 } // Souris Gaming: 49.99 * 2 = 99.98
            }
        };

        // Act
        var (response, errors) = orderService.ProcessOrder(request);

        // Assert
        Assert.Empty(errors);
        Assert.NotNull(response);
        Assert.Equal(99.98m, response.Total);
    }

    [Fact]
    public void ProcessOrder_QuantityOver5_ShouldApply10PercentDiscount()
    {
        // Arrange
        var stockService = CreateStockService();
        var orderService = new OrderService(stockService);
        var request = new OrderRequest
        {
            Products = new List<OrderProductRequest>
            {
                new() { Id = 2, Quantity = 6 } // Souris Gaming: 49.99 * 6 = 299.94, avec 10% = 269.95
            }
        };

        // Act
        var (response, errors) = orderService.ProcessOrder(request);

        // Assert
        Assert.Empty(errors);
        Assert.NotNull(response);
        // Total produit: 49.99 * 6 * 0.9 = 269.946 arrondi à 269.95
        // Puis remise de 5% car > 100€: 269.95 * 0.95 = 256.4525 arrondi à 256.45
        var expectedProductTotal = Math.Round(49.99m * 6 * 0.90m, 2);
        Assert.Equal(expectedProductTotal, response.Products[0].Total);
    }

    [Fact]
    public void ProcessOrder_TotalOver100_ShouldApply5PercentOrderDiscount()
    {
        // Arrange
        var stockService = CreateStockService();
        var orderService = new OrderService(stockService);
        var request = new OrderRequest
        {
            Products = new List<OrderProductRequest>
            {
                new() { Id = 3, Quantity = 1 } // Clavier Mécanique: 129.99
            }
        };

        // Act
        var (response, errors) = orderService.ProcessOrder(request);

        // Assert
        Assert.Empty(errors);
        Assert.NotNull(response);
        Assert.Single(response.Discounts);
        Assert.Equal("order", response.Discounts[0].Type);
        // Total: 129.99 * 0.95 = 123.4905 arrondi à 123.49
        Assert.Equal(Math.Round(129.99m * 0.95m, 2), response.Total);
    }

    [Fact]
    public void ProcessOrder_TotalUnder100_ShouldNotApplyOrderDiscount()
    {
        // Arrange
        var stockService = CreateStockService();
        var orderService = new OrderService(stockService);
        var request = new OrderRequest
        {
            Products = new List<OrderProductRequest>
            {
                new() { Id = 2, Quantity = 1 } // Souris Gaming: 49.99
            }
        };

        // Act
        var (response, errors) = orderService.ProcessOrder(request);

        // Assert
        Assert.Empty(errors);
        Assert.NotNull(response);
        Assert.Empty(response.Discounts);
        Assert.Equal(49.99m, response.Total);
    }

    #endregion

    #region Tests de gestion des stocks (Règle b)

    [Fact]
    public void ProcessOrder_InsufficientStock_ShouldReturnError()
    {
        // Arrange
        var stockService = CreateStockService();
        var orderService = new OrderService(stockService);
        var product = stockService.GetProductById(4); // Écran 27 pouces: stock = 10
        var request = new OrderRequest
        {
            Products = new List<OrderProductRequest>
            {
                new() { Id = 4, Quantity = 100 } // Quantité supérieure au stock
            }
        };

        // Act
        var (response, errors) = orderService.ProcessOrder(request);

        // Assert
        Assert.Null(response);
        Assert.Single(errors);
        Assert.Contains($"Il ne reste que {product!.Stock} exemplaire(s) pour le produit {product.Name}", errors);
    }

    [Fact]
    public void ProcessOrder_SuccessfulOrder_ShouldUpdateStock()
    {
        // Arrange
        var stockService = CreateStockService();
        var orderService = new OrderService(stockService);
        var initialProduct = stockService.GetProductById(2);
        var initialStock = initialProduct!.Stock;
        var request = new OrderRequest
        {
            Products = new List<OrderProductRequest>
            {
                new() { Id = 2, Quantity = 3 }
            }
        };

        // Act
        var (response, errors) = orderService.ProcessOrder(request);

        // Assert
        Assert.Empty(errors);
        var updatedProduct = stockService.GetProductById(2);
        Assert.Equal(initialStock - 3, updatedProduct!.Stock);
    }

    #endregion

    #region Tests des codes promos (Règle c)

    [Fact]
    public void ProcessOrder_ValidPromoCode_ShouldApplyDiscount()
    {
        // Arrange
        var stockService = CreateStockService();
        var orderService = new OrderService(stockService);
        var request = new OrderRequest
        {
            Products = new List<OrderProductRequest>
            {
                new() { Id = 5, Quantity = 1 } // Casque Audio: 79.99 (> 50€)
            },
            PromoCode = "DISCOUNT20"
        };

        // Act
        var (response, errors) = orderService.ProcessOrder(request);

        // Assert
        Assert.Empty(errors);
        Assert.NotNull(response);
        Assert.Single(response.Discounts);
        Assert.Equal("promo", response.Discounts[0].Type);
        // Total: 79.99 * (100-20)/100 = 79.99 * 0.80 = 63.992 arrondi à 63.99
        Assert.Equal(Math.Round(79.99m * 0.80m, 2), response.Total);
    }

    [Fact]
    public void ProcessOrder_InvalidPromoCode_ShouldReturnError()
    {
        // Arrange
        var stockService = CreateStockService();
        var orderService = new OrderService(stockService);
        var request = new OrderRequest
        {
            Products = new List<OrderProductRequest>
            {
                new() { Id = 5, Quantity = 1 } // Casque Audio: 79.99
            },
            PromoCode = "INVALID_CODE"
        };

        // Act
        var (response, errors) = orderService.ProcessOrder(request);

        // Assert
        Assert.Null(response);
        Assert.Single(errors);
        Assert.Equal("Le code promo est invalide", errors[0]);
    }

    [Fact]
    public void ProcessOrder_PromoCodeUnder50_ShouldReturnError()
    {
        // Arrange
        var stockService = CreateStockService();
        var orderService = new OrderService(stockService);
        var request = new OrderRequest
        {
            Products = new List<OrderProductRequest>
            {
                new() { Id = 2, Quantity = 1 } // Souris Gaming: 49.99 (< 50€)
            },
            PromoCode = "DISCOUNT20"
        };

        // Act
        var (response, errors) = orderService.ProcessOrder(request);

        // Assert
        Assert.Null(response);
        Assert.Single(errors);
        Assert.Equal("Les codes promos ne sont valables qu'à partir de 50 euros d'achat", errors[0]);
    }

    [Fact]
    public void ProcessOrder_PromoCodeAndOrderDiscount_ShouldCumulateAdditively()
    {
        // Arrange - Commande > 100€ avec code promo DISCOUNT10
        // Total avant remises: 200€
        // Remise order: 5% + Remise promo: 10% = 15% de réduction
        // Total final: 200 * (100-15)/100 = 200 * 0.85 = 170€
        var stockService = CreateStockService();
        var orderService = new OrderService(stockService);
        var request = new OrderRequest
        {
            Products = new List<OrderProductRequest>
            {
                new() { Id = 3, Quantity = 2 } // Clavier Mécanique: 129.99 * 2 = 259.98
            },
            PromoCode = "DISCOUNT10"
        };

        // Act
        var (response, errors) = orderService.ProcessOrder(request);

        // Assert
        Assert.Empty(errors);
        Assert.NotNull(response);
        Assert.Equal(2, response.Discounts.Count);
        
        // Vérifier les deux types de remises
        Assert.Contains(response.Discounts, d => d.Type == "order");
        Assert.Contains(response.Discounts, d => d.Type == "promo");
        
        // Total: 259.98 * (100-5-10)/100 = 259.98 * 0.85 = 220.983 arrondi à 220.98
        Assert.Equal(Math.Round(259.98m * 0.85m, 2), response.Total);
    }

    [Fact]
    public void ProcessOrder_Discount20AndOrderDiscount_ShouldCumulate()
    {
        // Arrange
        var stockService = CreateStockService();
        var orderService = new OrderService(stockService);
        var request = new OrderRequest
        {
            Products = new List<OrderProductRequest>
            {
                new() { Id = 3, Quantity = 2 } // Clavier Mécanique: 129.99 * 2 = 259.98
            },
            PromoCode = "DISCOUNT20"
        };

        // Act
        var (response, errors) = orderService.ProcessOrder(request);

        // Assert
        Assert.Empty(errors);
        Assert.NotNull(response);
        // Total: 259.98 * (100-5-20)/100 = 259.98 * 0.75 = 194.985 arrondi à 194.99
        Assert.Equal(Math.Round(259.98m * 0.75m, 2), response.Total);
    }

    #endregion

    #region Tests de validation des produits (Règle d)

    [Fact]
    public void ProcessOrder_NonExistingProduct_ShouldReturnError()
    {
        // Arrange
        var stockService = CreateStockService();
        var orderService = new OrderService(stockService);
        var request = new OrderRequest
        {
            Products = new List<OrderProductRequest>
            {
                new() { Id = 9999, Quantity = 1 }
            }
        };

        // Act
        var (response, errors) = orderService.ProcessOrder(request);

        // Assert
        Assert.Null(response);
        Assert.Single(errors);
        Assert.Equal("Le produit avec l'identifiant 9999 n'existe pas", errors[0]);
    }

    #endregion

    #region Tests avec plusieurs erreurs

    [Fact]
    public void ProcessOrder_MultipleErrors_ShouldReturnAllErrors()
    {
        // Arrange
        var stockService = CreateStockService();
        var orderService = new OrderService(stockService);
        var product = stockService.GetProductById(4); // Écran: stock = 10
        var request = new OrderRequest
        {
            Products = new List<OrderProductRequest>
            {
                new() { Id = 9999, Quantity = 1 }, // Produit inexistant
                new() { Id = 4, Quantity = 100 }  // Stock insuffisant
            }
        };

        // Act
        var (response, errors) = orderService.ProcessOrder(request);

        // Assert
        Assert.Null(response);
        Assert.Equal(2, errors.Count);
        Assert.Contains("Le produit avec l'identifiant 9999 n'existe pas", errors);
        Assert.Contains($"Il ne reste que {product!.Stock} exemplaire(s) pour le produit {product.Name}", errors);
    }

    [Fact]
    public void ProcessOrder_MultipleNonExistingProducts_ShouldReturnAllErrors()
    {
        // Arrange
        var stockService = CreateStockService();
        var orderService = new OrderService(stockService);
        var request = new OrderRequest
        {
            Products = new List<OrderProductRequest>
            {
                new() { Id = 1111, Quantity = 1 },
                new() { Id = 2222, Quantity = 1 },
                new() { Id = 3333, Quantity = 1 }
            }
        };

        // Act
        var (response, errors) = orderService.ProcessOrder(request);

        // Assert
        Assert.Null(response);
        Assert.Equal(3, errors.Count);
        Assert.Contains("Le produit avec l'identifiant 1111 n'existe pas", errors);
        Assert.Contains("Le produit avec l'identifiant 2222 n'existe pas", errors);
        Assert.Contains("Le produit avec l'identifiant 3333 n'existe pas", errors);
    }

    #endregion

    #region Tests d'intégration complets

    [Fact]
    public void ProcessOrder_CompleteScenario_ShouldWorkCorrectly()
    {
        // Arrange - Scénario de l'énoncé
        var stockService = CreateStockService();
        var orderService = new OrderService(stockService);
        var request = new OrderRequest
        {
            Products = new List<OrderProductRequest>
            {
                new() { Id = 2, Quantity = 3 }, // Souris Gaming: 49.99 * 3 = 149.97
                new() { Id = 5, Quantity = 2 }  // Casque Audio: 79.99 * 2 = 159.98
            },
            PromoCode = "DISCOUNT20"
        };

        // Act
        var (response, errors) = orderService.ProcessOrder(request);

        // Assert
        Assert.Empty(errors);
        Assert.NotNull(response);
        Assert.Equal(2, response.Products.Count);
        
        // Vérifier les remises
        Assert.Equal(2, response.Discounts.Count);
        Assert.Contains(response.Discounts, d => d.Type == "order");
        Assert.Contains(response.Discounts, d => d.Type == "promo");

        // Total avant remises: 149.97 + 159.98 = 309.95
        // Avec remises cumulées (5% + 20% = 25%): 309.95 * 0.75 = 232.4625 arrondi à 232.46
        Assert.Equal(Math.Round(309.95m * 0.75m, 2), response.Total);
    }

    [Fact]
    public void ProcessOrder_WithQuantityDiscount_ShouldApplyCorrectly()
    {
        // Arrange - Commande avec quantité > 5 pour un produit
        var stockService = CreateStockService();
        var orderService = new OrderService(stockService);
        var request = new OrderRequest
        {
            Products = new List<OrderProductRequest>
            {
                new() { Id = 2, Quantity = 10 } // Souris Gaming: 49.99 * 10 = 499.90, avec -10% = 449.91
            }
        };

        // Act
        var (response, errors) = orderService.ProcessOrder(request);

        // Assert
        Assert.Empty(errors);
        Assert.NotNull(response);
        
        // Le total du produit devrait avoir la remise de 10%
        var expectedProductTotal = Math.Round(49.99m * 10 * 0.90m, 2);
        Assert.Equal(expectedProductTotal, response.Products[0].Total);
        
        // Le total final devrait aussi avoir la remise de 5% car > 100€
        Assert.Single(response.Discounts);
        Assert.Equal("order", response.Discounts[0].Type);
    }

    #endregion
}
