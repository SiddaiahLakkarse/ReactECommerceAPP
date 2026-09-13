namespace ECommerce.Domain;

public sealed class Product
{
    private Product() { }

    public Product(string name, string description, decimal price, int stockQuantity, Guid? id = null)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Product name is required.", nameof(name));
        if (price < 0) throw new ArgumentOutOfRangeException(nameof(price));
        if (stockQuantity < 0) throw new ArgumentOutOfRangeException(nameof(stockQuantity));

        Id = id ?? Guid.NewGuid();
        Name = name.Trim();
        Description = description?.Trim() ?? string.Empty;
        Price = price;
        StockQuantity = stockQuantity;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int StockQuantity { get; private set; }
    public bool IsActive { get; private set; } = true;

    public void DecreaseStock(int quantity)
    {
        if (quantity <= 0 || quantity > StockQuantity) throw new InvalidOperationException("Insufficient stock.");
        StockQuantity -= quantity;
    }
}

public sealed class Cart
{
    private Cart() { }
    public Cart(Guid userId) { Id = Guid.NewGuid(); UserId = userId; }
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public List<CartItem> Items { get; private set; } = [];
}

public sealed class CartItem
{
    private CartItem() { }
    public CartItem(Guid productId, int quantity) { ProductId = productId; Quantity = quantity; }
    public Guid Id { get; private set; }
    public Guid CartId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public void SetQuantity(int quantity) => Quantity = quantity > 0 ? quantity : throw new ArgumentOutOfRangeException(nameof(quantity));
}

public enum OrderStatus { Pending, Paid, Shipped, Completed, Cancelled }

public sealed class Order
{
    private Order() { }
    public Order(Guid userId, IEnumerable<OrderItem> items)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Items = items.ToList();
        Total = Items.Sum(x => x.UnitPrice * x.Quantity);
        Status = OrderStatus.Pending;
        CreatedUtc = DateTime.UtcNow;
    }
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public decimal Total { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedUtc { get; private set; }
    public List<OrderItem> Items { get; private set; } = [];
}

public sealed class OrderItem
{
    private OrderItem() { }
    public OrderItem(Guid productId, string productName, decimal unitPrice, int quantity)
    {
        ProductId = productId; ProductName = productName; UnitPrice = unitPrice; Quantity = quantity;
    }
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }
}
