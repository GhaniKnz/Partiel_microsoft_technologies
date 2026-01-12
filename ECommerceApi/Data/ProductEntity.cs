using System.ComponentModel.DataAnnotations;

namespace ECommerceApi.Data;

// Entité produit pour la table Products
public class ProductEntity
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    public decimal Price { get; set; }
    
    public int Stock { get; set; }
}
