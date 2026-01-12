using ECommerceApi.Models;
using ECommerceApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApi.Controllers;

/// <summary>
/// Contrôleur pour la gestion des produits
/// </summary>
[ApiController]
[Route("[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IStockService _stockService;

    public ProductsController(IStockService stockService)
    {
        _stockService = stockService;
    }

    /// <summary>
    /// GET /Products - Retourne la liste de tous les produits
    /// </summary>
    [HttpGet]
    public ActionResult<List<ProductDto>> GetProducts()
    {
        var products = _stockService.GetAllProducts();
        
        var productDtos = products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            Stock = p.Stock
        }).ToList();

        return Ok(productDtos);
    }
}
