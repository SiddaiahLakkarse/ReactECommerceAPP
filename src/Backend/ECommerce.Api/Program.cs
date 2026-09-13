using ECommerce.Application;
using ECommerce.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ECommerceDbContext>(options =>
{
    if (builder.Environment.IsDevelopment())
        options.UseInMemoryDatabase("ECommerceDevelopment");
    else
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ECommerceDbContext>());
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ProductCommands>();
builder.Services.AddScoped<CartCommands>();
builder.Services.AddScoped<OrderCommands>();
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ECommerceDbContext>();
    db.Database.EnsureCreated();
    if (!db.Products.Any())
    {
        db.Products.AddRange(
            new ECommerce.Domain.Product("Stoneware Mug", "Hand-finished ceramic for your morning ritual.", 24.00m, 18),
            new ECommerce.Domain.Product("Linen Throw", "Soft washed linen in a warm natural tone.", 68.00m, 12),
            new ECommerce.Domain.Product("Field Notebook", "Hard-wearing recycled paper for good ideas.", 14.00m, 30));
        db.SaveChanges();
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapControllers();
app.Run();
