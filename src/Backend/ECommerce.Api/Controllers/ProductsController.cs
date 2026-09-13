using ECommerce.Application;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/products")]
public sealed class ProductsController(IProductRepository products, ProductCommands commands) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>> List(CancellationToken cancellationToken) => Ok((await products.ListAsync(cancellationToken)).Select(x => new ProductDto(x.Id, x.Name, x.Description, x.Price, x.StockQuantity)));

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(CreateProductCommand command, CancellationToken cancellationToken) => CreatedAtAction(nameof(List), await commands.Handle(command, cancellationToken));
}