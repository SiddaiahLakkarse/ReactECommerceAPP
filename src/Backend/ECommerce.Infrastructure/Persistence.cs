using ECommerce.Application;
using ECommerce.Domain;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure;

public sealed class ECommerceDbContext(DbContextOptions<ECommerceDbContext> options) : DbContext(options), IUnitOfWork
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasKey(x => x.Id);
        modelBuilder.Entity<Product>().Property(x => x.Price).HasPrecision(18, 2);
        modelBuilder.Entity<Product>().HasIndex(x => new { x.IsActive, x.Name });
        modelBuilder.Entity<Cart>().HasIndex(x => x.UserId).IsUnique();
        modelBuilder.Entity<CartItem>().HasIndex(x => new { x.CartId, x.ProductId }).IsUnique();
        modelBuilder.Entity<Order>().Property(x => x.Total).HasPrecision(18, 2);
        modelBuilder.Entity<Order>().HasIndex(x => new { x.UserId, x.CreatedUtc });
        modelBuilder.Entity<OrderItem>().Property(x => x.UnitPrice).HasPrecision(18, 2);
        modelBuilder.Entity<Cart>().HasMany(x => x.Items).WithOne().HasForeignKey(x => x.CartId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Order>().HasMany(x => x.Items).WithOne().HasForeignKey(x => x.OrderId).OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class ProductRepository(ECommerceDbContext db) : IProductRepository
{
    public Task<IReadOnlyList<Product>> ListAsync(CancellationToken cancellationToken) => db.Products.AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.Name).ToListAsync(cancellationToken).ContinueWith(x => (IReadOnlyList<Product>)x.Result, cancellationToken);
    public Task<Product?> GetAsync(Guid id, CancellationToken cancellationToken) => db.Products.SingleOrDefaultAsync(x => x.Id == id && x.IsActive, cancellationToken);
    public Task AddAsync(Product product, CancellationToken cancellationToken) => db.Products.AddAsync(product, cancellationToken).AsTask();
}

public sealed class CartRepository(ECommerceDbContext db) : ICartRepository
{
    public async Task<Cart> GetOrCreateAsync(Guid userId, CancellationToken cancellationToken) => await db.Carts.Include(x => x.Items).SingleOrDefaultAsync(x => x.UserId == userId, cancellationToken) ?? new Cart(userId);
    public Task SaveAsync(Cart cart, CancellationToken cancellationToken) { db.Carts.Update(cart); return Task.CompletedTask; }
}

public sealed class OrderRepository(ECommerceDbContext db) : IOrderRepository
{
    public Task AddAsync(Order order, CancellationToken cancellationToken) => db.Orders.AddAsync(order, cancellationToken).AsTask();
    public async Task<IReadOnlyList<Order>> ListByUserAsync(Guid userId, CancellationToken cancellationToken) => await db.Orders.AsNoTracking().Where(x => x.UserId == userId).OrderByDescending(x => x.CreatedUtc).ToListAsync(cancellationToken);
}