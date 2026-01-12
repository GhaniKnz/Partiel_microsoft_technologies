using ECommerceApi.Models;

namespace ECommerceApi.Services;

// Interface pour le service de commandes
public interface IOrderService
{
    (OrderResponse? Response, List<string> Errors) ProcessOrder(OrderRequest request);
}

// Service qui gère la logique métier des commandes
public class OrderService : IOrderService
{
    private readonly IStockService _stockService;

    // Codes promos disponibles avec leur pourcentage de réduction
    private static readonly Dictionary<string, decimal> PromoCodes = new(StringComparer.OrdinalIgnoreCase)
    {
        { "DISCOUNT20", 20m },
        { "DISCOUNT10", 10m }
    };

    public OrderService(IStockService stockService)
    {
        _stockService = stockService;
    }

    public (OrderResponse? Response, List<string> Errors) ProcessOrder(OrderRequest request)
    {
        var errors = new List<string>();
        var orderProducts = new List<OrderProductResponse>();
        decimal totalBeforeDiscounts = 0;

        // Étape 1 : Validation des produits et calcul des totaux
        foreach (var item in request.Products)
        {
            var product = _stockService.GetProductById(item.Id);

            // Règle d.1 : Vérifier si le produit existe
            if (product == null)
            {
                errors.Add($"Le produit avec l'identifiant {item.Id} n'existe pas");
                continue;
            }

            // Règle b.1 : Vérifier le stock disponible
            if (product.Stock < item.Quantity)
            {
                errors.Add($"Il ne reste que {product.Stock} exemplaire(s) pour le produit {product.Name}");
                continue;
            }

            // Calcul du prix pour ce produit
            decimal productTotal = product.Price * item.Quantity;

            // Règle a.1 : Remise de 10% si quantité > 5 (remise silencieuse, non affichée)
            if (item.Quantity > 5)
            {
                productTotal *= 0.90m; // 10% de réduction
            }

            totalBeforeDiscounts += productTotal;

            orderProducts.Add(new OrderProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Quantity = item.Quantity,
                PricePerUnit = product.Price,
                Total = Math.Round(productTotal, 2)
            });
        }

        // Si des erreurs ont été détectées, on retourne les erreurs
        if (errors.Any())
        {
            return (null, errors);
        }

        // Étape 2 : Gestion des codes promos
        decimal promoDiscountPercent = 0;
        decimal promoDiscountValue = 0;

        if (!string.IsNullOrWhiteSpace(request.PromoCode))
        {
            // Règle c.1 : Vérifier si le code promo existe
            if (!PromoCodes.TryGetValue(request.PromoCode, out promoDiscountPercent))
            {
                errors.Add("Le code promo est invalide");
                return (null, errors);
            }

            // Règle c.2 : Les codes promos sont valides seulement si la commande dépasse 50 euros
            if (totalBeforeDiscounts < 50)
            {
                errors.Add("Les codes promos ne sont valables qu'à partir de 50 euros d'achat");
                return (null, errors);
            }
        }

        // Étape 3 : Calcul des remises et du total final
        var discounts = new List<Discount>();
        decimal totalDiscountPercent = 0;

        // Règle a.2 : Remise de 5% si total > 100 euros (type "order")
        decimal orderDiscountValue = 0;
        if (totalBeforeDiscounts > 100)
        {
            totalDiscountPercent += 5;
            orderDiscountValue = Math.Round(totalBeforeDiscounts * 0.05m, 2);
            discounts.Add(new Discount { Type = "order", Value = orderDiscountValue });
        }

        // Règle c.3 : Les codes promos se cumulent avec la promotion de 5%, de façon additive
        if (promoDiscountPercent > 0)
        {
            totalDiscountPercent += promoDiscountPercent;
            promoDiscountValue = Math.Round(totalBeforeDiscounts * (promoDiscountPercent / 100m), 2);
            discounts.Add(new Discount { Type = "promo", Value = promoDiscountValue });
        }

        // Calcul du total final avec les remises additives
        decimal finalTotal = totalBeforeDiscounts * ((100 - totalDiscountPercent) / 100m);
        finalTotal = Math.Round(finalTotal, 2);

        // Étape 4 : Mise à jour des stocks (Règle b.2)
        foreach (var item in request.Products)
        {
            _stockService.UpdateStock(item.Id, item.Quantity);
        }

        // Construction de la réponse
        var response = new OrderResponse
        {
            Products = orderProducts,
            Discounts = discounts,
            Total = finalTotal
        };

        return (response, errors);
    }
}
