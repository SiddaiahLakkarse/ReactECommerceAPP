using System.Security.Claims;
using ECommerce.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/customer")]
public sealed class CustomerController(ICartRepository carts, IProductRepository products, IOrderRepository orders, CartCommands cartCommands) : ControllerBase
{
    [HttpGet("cart")]
    public async Task<IActionResult> Cart(CancellationToken cancellationToken)
    {
        var cart = await carts.GetOrCreateAsync(UserId(), cancellationToken);
        var items = new List<object>();
        foreach (var item in cart.Items)
        {
            var product = await products.GetAsync(item.ProductId, cancellationToken);
            if (product is not null) items.Add(new { product.Id, product.Name, product.Description, product.Price, item.Quantity });
        }
        return Ok(items);
    }

    [HttpPost("cart/items")]
    public async Task<IActionResult> AddToCart(AddItemRequest request, CancellationToken cancellationToken)
    {
        await cartCommands.Handle(new AddToCartCommand(UserId(), request.ProductId, request.Quantity), cancellationToken);
        return NoContent();
    }

    [HttpGet("orders")]
    public async Task<IActionResult> Orders(CancellationToken cancellationToken) => Ok(await orders.ListByUserAsync(UserId(), cancellationToken));

    private Guid UserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Name) ?? throw new UnauthorizedAccessException());
    public sealed record AddItemRequest(Guid ProductId, int Quantity);
}