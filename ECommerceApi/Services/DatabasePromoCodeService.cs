using ECommerceApi.Data;

namespace ECommerceApi.Services;

/// <summary>
/// Service de gestion des codes promos utilisant la base de données
/// </summary>
public class DatabasePromoCodeService : IPromoCodeService
{
    private readonly ECommerceDbContext _context;

    public DatabasePromoCodeService(ECommerceDbContext context)
    {
        _context = context;
    }

    public decimal? GetDiscountPercent(string code)
    {
        var promo = _context.PromoCodes
            .FirstOrDefault(p => p.Code.ToUpper() == code.ToUpper());
        
        return promo?.DiscountPercent;
    }

    public bool IsValidCode(string code)
    {
        return _context.PromoCodes
            .Any(p => p.Code.ToUpper() == code.ToUpper());
    }
}
