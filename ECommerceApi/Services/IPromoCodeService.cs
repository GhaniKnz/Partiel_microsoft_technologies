namespace ECommerceApi.Services;

/// <summary>
/// Interface pour le service de gestion des codes promos
/// </summary>
public interface IPromoCodeService
{
    /// <summary>
    /// Récupère le pourcentage de réduction d'un code promo
    /// </summary>
    /// <param name="code">Code promo</param>
    /// <returns>Le pourcentage ou null si le code n'existe pas</returns>
    decimal? GetDiscountPercent(string code);

    /// <summary>
    /// Vérifie si un code promo existe
    /// </summary>
    bool IsValidCode(string code);
}
