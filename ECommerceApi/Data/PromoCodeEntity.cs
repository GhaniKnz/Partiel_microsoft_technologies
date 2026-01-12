using System.ComponentModel.DataAnnotations;

namespace ECommerceApi.Data;

// Entité code promo pour la table PromoCodes
public class PromoCodeEntity
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;
    
    // Pourcentage de réduction (ex: 20 pour 20%)
    public decimal DiscountPercent { get; set; }
}
