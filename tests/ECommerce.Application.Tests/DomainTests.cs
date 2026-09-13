using ECommerce.Domain;

namespace ECommerce.Application.Tests;

public class DomainTests
{
    [Fact]
    public void Product_decreases_stock_when_quantity_is_available()
    {
        var product = new Product("Keyboard", "Mechanical keyboard", 99, 5);
        product.DecreaseStock(2);
        Assert.Equal(3, product.StockQuantity);
    }

    [Fact]
    public void Product_rejects_insufficient_stock()
    {
        var product = new Product("Keyboard", "Mechanical keyboard", 99, 1);
        Assert.Throws<InvalidOperationException>(() => product.DecreaseStock(2));
    }
}
