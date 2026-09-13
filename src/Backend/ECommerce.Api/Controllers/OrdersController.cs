using ECommerce.Application;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/orders")]
public sealed class OrdersController(OrderCommands commands) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create(CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException());
        return Ok(await commands.Handle(new CreateOrderCommand(userId), cancellationToken));
    }
}