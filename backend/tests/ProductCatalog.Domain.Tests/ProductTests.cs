using FluentAssertions;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Exceptions;
using Xunit;

namespace ProductCatalog.Domain.Tests;

public class ProductTests
{
    [Fact]
    public void Product_Create_ShouldInstantiateProduct_WhenParametersAreValid()
    {
        // Arrange
        var sku = "LAP-DELL-XPS15";
        var name = "Dell XPS 15";
        var description = "Portátil potente";
        var price = 2499.99m;
        var initialStock = 10;

        // Act
        var product = Product.Create(sku, name, description, price, initialStock);

        // Assert
        product.Should().NotBeNull();
        product.Id.Should().NotBeEmpty();
        product.Sku.Should().Be("LAP-DELL-XPS15");
        product.Name.Should().Be("Dell XPS 15");
        product.Description.Should().Be("Portátil potente");
        product.Price.Should().Be(price);
        product.Stock.Should().Be(initialStock);
        product.CreatedAtUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        product.UpdatedAtUtc.Should().BeNull();
    }

    [Theory]
    [InlineData(0.00)]
    [InlineData(-10.50)]
    public void Product_Create_ShouldThrowDomainRuleValidationException_WhenPriceIsZeroOrNegative(decimal invalidPrice)
    {
        // Act
        Action act = () => Product.Create("SKU-100", "Producto Test", "Desc", invalidPrice, 5);

        // Assert
        act.Should().Throw<DomainRuleValidationException>()
            .WithMessage("*precio unitario debe ser estrictamente mayor a cero*");
    }

    [Fact]
    public void Product_Create_ShouldThrowDomainRuleValidationException_WhenInitialStockIsNegative()
    {
        // Act
        Action act = () => Product.Create("SKU-100", "Producto Test", "Desc", 100m, -1);

        // Assert
        act.Should().Throw<DomainRuleValidationException>()
            .WithMessage("*stock inicial no puede ser negativo*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Product_Create_ShouldThrowDomainRuleValidationException_WhenSkuIsEmpty(string? invalidSku)
    {
        // Act
        Action act = () => Product.Create(invalidSku!, "Producto Test", "Desc", 100m, 5);

        // Assert
        act.Should().Throw<DomainRuleValidationException>()
            .WithMessage("*código SKU del producto es obligatorio*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Product_Create_ShouldThrowDomainRuleValidationException_WhenNameIsEmpty(string? invalidName)
    {
        // Act
        Action act = () => Product.Create("SKU-100", invalidName!, "Desc", 100m, 5);

        // Assert
        act.Should().Throw<DomainRuleValidationException>()
            .WithMessage("*nombre del producto es obligatorio*");
    }

    [Fact]
    public void Product_AdjustStock_ShouldIncreaseStock_WhenDeltaIsPositive()
    {
        // Arrange
        var product = Product.Create("SKU-100", "Producto Test", null, 50m, 10);

        // Act
        var resultingStock = product.AdjustStock(5);

        // Assert
        resultingStock.Should().Be(15);
        product.Stock.Should().Be(15);
        product.UpdatedAtUtc.Should().NotBeNull();
        product.UpdatedAtUtc!.Value.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void Product_AdjustStock_ShouldDecreaseStock_WhenDeltaIsNegativeAndSufficientStock()
    {
        // Arrange
        var product = Product.Create("SKU-100", "Producto Test", null, 50m, 10);

        // Act
        var resultingStock = product.AdjustStock(-4);

        // Assert
        resultingStock.Should().Be(6);
        product.Stock.Should().Be(6);
        product.UpdatedAtUtc.Should().NotBeNull();
    }

    [Fact]
    public void Product_AdjustStock_ShouldThrowDomainRuleValidationException_WhenResultingStockIsNegative()
    {
        // Arrange
        var product = Product.Create("SKU-100", "Producto Test", null, 50m, 5);

        // Act
        Action act = () => product.AdjustStock(-6);

        // Assert
        act.Should().Throw<DomainRuleValidationException>()
            .WithMessage("*dejaría el stock en negativo*");

        product.Stock.Should().Be(5); // Invariante: el estado no se modifica ante fallo
    }
}
