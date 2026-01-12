using ECommerceApi.Models;
using ECommerceApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApi.Controllers;

/// <summary>
/// Contrôleur pour la gestion des commandes
/// </summary>
[ApiController]
[Route("[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>
    /// POST /orders - Crée une nouvelle commande
    /// </summary>
    [HttpPost]
    public ActionResult CreateOrder([FromBody] OrderRequest request)
    {
        if (request == null || request.Products == null || !request.Products.Any())
        {
            return BadRequest(new ErrorResponse
            {
                Errors = new List<string> { "La commande doit contenir au moins un produit" }
            });
        }

        var (response, errors) = _orderService.ProcessOrder(request);

        if (errors.Any())
        {
            return BadRequest(new ErrorResponse { Errors = errors });
        }

        return Ok(response);
    }
}
