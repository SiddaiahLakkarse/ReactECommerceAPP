using ECommerce.Application;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/orders")]
public sealed class OrdersController(OrderCommands commands) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create(CreateOrderCommand command, CancellationToken cancellationToken) => Ok(await commands.Handle(command, cancellationToken));
}