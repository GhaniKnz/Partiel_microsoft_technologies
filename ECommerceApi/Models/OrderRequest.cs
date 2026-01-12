using System.Text.Json.Serialization;

namespace ECommerceApi.Models;

/// <summary>
/// Requête d'achat - payload d'entrée pour POST /orders
/// </summary>
public class OrderRequest
{
    [JsonPropertyName("products")]
    public List<OrderProductRequest> Products { get; set; } = new();

    [JsonPropertyName("promo_code")]
    public string? PromoCode { get; set; }
}

/// <summary>
/// Produit dans la requête de commande
/// </summary>
public class OrderProductRequest
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }
}
