using ECommerce.Application;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/cart")]
public sealed class CartController(CartCommands commands) : ControllerBase
{
    [HttpPost("items")]
    public async Task<IActionResult> Add(AddToCartCommand command, CancellationToken cancellationToken)
    {
        await commands.Handle(command, cancellationToken);
        return NoContent();
    }
}