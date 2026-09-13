using ECommerce.Domain;

namespace ECommerce.Application;

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> ListAsync(CancellationToken cancellationToken);
    Task<Product?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(Product product, CancellationToken cancellationToken);
}

public interface ICartRepository
{
    Task<Cart> GetOrCreateAsync(Guid userId, CancellationToken cancellationToken);
    Task SaveAsync(Cart cart, CancellationToken cancellationToken);
}

public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken cancellationToken);
    Task<IReadOnlyList<Order>> ListByUserAsync(Guid userId, CancellationToken cancellationToken);
}

public interface IUnitOfWork { Task<int> SaveChangesAsync(CancellationToken cancellationToken); }

public sealed record ProductDto(Guid Id, string Name, string Description, decimal Price, int StockQuantity);
public sealed record OrderDto(Guid Id, decimal Total, string Status, DateTime CreatedUtc);

public sealed record CreateProductCommand(string Name, string Description, decimal Price, int StockQuantity);
public sealed record AddToCartCommand(Guid UserId, Guid ProductId, int Quantity);
public sealed record CreateOrderCommand(Guid UserId);

public sealed class ProductCommands(IProductRepository products, IUnitOfWork unitOfWork)
{
    public async Task<ProductDto> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var product = new Product(command.Name, command.Description, command.Price, command.StockQuantity);
        await products.AddAsync(product, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new(product.Id, product.Name, product.Description, product.Price, product.StockQuantity);
    }
}

public sealed class CartCommands(ICartRepository carts, IProductRepository products, IUnitOfWork unitOfWork)
{
    public async Task Handle(AddToCartCommand command, CancellationToken cancellationToken)
    {
        var product = await products.GetAsync(command.ProductId, cancellationToken) ?? throw new KeyNotFoundException("Product not found.");
        if (command.Quantity <= 0 || command.Quantity > product.StockQuantity) throw new InvalidOperationException("Requested quantity is unavailable.");
        var cart = await carts.GetOrCreateAsync(command.UserId, cancellationToken);
        var item = cart.Items.SingleOrDefault(x => x.ProductId == command.ProductId);
        if (item is null) cart.Items.Add(new CartItem(command.ProductId, command.Quantity));
        else item.SetQuantity(item.Quantity + command.Quantity);
        await carts.SaveAsync(cart, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

public sealed class OrderCommands(ICartRepository carts, IProductRepository products, IOrderRepository orders, IUnitOfWork unitOfWork)
{
    public async Task<OrderDto> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var cart = await carts.GetOrCreateAsync(command.UserId, cancellationToken);
        if (cart.Items.Count == 0) throw new InvalidOperationException("The cart is empty.");
        var items = new List<OrderItem>();
        foreach (var cartItem in cart.Items)
        {
            var product = await products.GetAsync(cartItem.ProductId, cancellationToken) ?? throw new KeyNotFoundException("Product not found.");
            product.DecreaseStock(cartItem.Quantity);
            items.Add(new OrderItem(product.Id, product.Name, product.Price, cartItem.Quantity));
        }
        var order = new Order(command.UserId, items);
        await orders.AddAsync(order, cancellationToken);
        cart.Items.Clear();
        await carts.SaveAsync(cart, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new(order.Id, order.Total, order.Status.ToString(), order.CreatedUtc);
    }
}