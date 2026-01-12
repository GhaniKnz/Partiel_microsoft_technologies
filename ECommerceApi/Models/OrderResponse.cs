using System.Text.Json.Serialization;

namespace ECommerceApi.Models;

// Modèle de réponse pour une commande réussie
public class OrderResponse
{
    [JsonPropertyName("products")]
    public List<OrderProductResponse> Products { get; set; } = new();

    [JsonPropertyName("discounts")]
    public List<Discount> Discounts { get; set; } = new();

    [JsonPropertyName("total")]
    public decimal Total { get; set; }
}

// Représente un produit dans la réponse de commande
public class OrderProductResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    [JsonPropertyName("price_per_unit")]
    public decimal PricePerUnit { get; set; }

    [JsonPropertyName("total")]
    public decimal Total { get; set; }
}

// Classe pour les remises appliquées
public class Discount
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("value")]
    public decimal Value { get; set; }
}

// Modèle pour les réponses d'erreur
public class ErrorResponse
{
    [JsonPropertyName("errors")]
    public List<string> Errors { get; set; } = new();
}
