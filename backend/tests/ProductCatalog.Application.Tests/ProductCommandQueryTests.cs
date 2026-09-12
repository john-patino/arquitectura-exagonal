using FluentAssertions;
using FluentValidation;
using NSubstitute;
using ProductCatalog.Application.Common.Behaviors;
using ProductCatalog.Application.Common.Interfaces;
using ProductCatalog.Application.Products.Commands.CreateProduct;
using ProductCatalog.Application.Products.Commands.UpdateProductStock;
using ProductCatalog.Application.Products.Queries.GetProductById;
using ProductCatalog.Application.Products.Queries.ListProducts;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Exceptions;
using Xunit;

namespace ProductCatalog.Application.Tests;

public class ProductCommandQueryTests
{
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    [Fact]
    public async Task CreateProductCommandHandler_ShouldPersistProduct_WhenValidationPassesAndSkuIsUnique()
    {
        // Arrange
        var command = new CreateProductCommand("MON-LG-34", "Monitor UltraWide LG", "34 pulgadas", 549.90m, 15);
        _productRepository.GetBySkuAsync("MON-LG-34", Arg.Any<CancellationToken>())
            .Returns((Product?)null);

        var handler = new CreateProductCommandHandler(_productRepository, _unitOfWork);

        // Act
        var resultId = await handler.Handle(command, CancellationToken.None);

        // Assert
        resultId.Should().NotBeEmpty();
        await _productRepository.Received(1).AddAsync(Arg.Is<Product>(p => p.Sku == "MON-LG-34" && p.Stock == 15), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateProductCommandHandler_ShouldThrowDomainRuleValidationException_WhenSkuAlreadyExists()
    {
        // Arrange
        var command = new CreateProductCommand("MON-LG-34", "Monitor UltraWide LG", null, 549.90m, 15);
        var existingProduct = Product.Create("MON-LG-34", "Existente", null, 500m, 2);

        _productRepository.GetBySkuAsync("MON-LG-34", Arg.Any<CancellationToken>())
            .Returns(existingProduct);

        var handler = new CreateProductCommandHandler(_productRepository, _unitOfWork);

        // Act
        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainRuleValidationException>()
            .WithMessage("*Ya existe un producto registrado con el SKU 'MON-LG-34'*");

        await _productRepository.DidNotReceive().AddAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateProductStockCommandHandler_ShouldCallAdjustStockAndCommit_WhenProductExists()
    {
        // Arrange
        var product = Product.Create("TEC-LOGI-MX", "Teclado Logitech", null, 150m, 20);
        _productRepository.GetByIdAsync(product.Id, Arg.Any<CancellationToken>())
            .Returns(product);

        var handler = new UpdateProductStockCommandHandler(_productRepository, _unitOfWork);
        var command = new UpdateProductStockCommand(product.Id, -5);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.ProductId.Should().Be(product.Id);
        result.CurrentStock.Should().Be(15);
        product.Stock.Should().Be(15);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetProductByIdQueryHandler_ShouldThrowProductNotFoundException_WhenProductDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        _productRepository.GetByIdAsync(nonExistentId, Arg.Any<CancellationToken>())
            .Returns((Product?)null);

        var handler = new GetProductByIdQueryHandler(_productRepository);
        var query = new GetProductByIdQuery(nonExistentId);

        // Act
        Func<Task> act = async () => await handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ProductNotFoundException>()
            .WithMessage($"*{nonExistentId}*");
    }

    [Fact]
    public async Task ListProductsQueryHandler_ShouldReturnMappedDtos()
    {
        // Arrange
        var products = new List<Product>
        {
            Product.Create("SKU-1", "Prod 1", null, 10m, 5),
            Product.Create("SKU-2", "Prod 2", null, 20m, 10)
        };

        _productRepository.ListAllAsync(Arg.Any<CancellationToken>())
            .Returns(products);

        var handler = new ListProductsQueryHandler(_productRepository);

        // Act
        var result = await handler.Handle(new ListProductsQuery(), CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result[0].Sku.Should().Be("SKU-1");
        result[1].Sku.Should().Be("SKU-2");
    }

    [Fact]
    public async Task ValidationBehavior_ShouldThrowValidationException_WhenCommandHasInvalidData()
    {
        // Arrange
        var validator = new CreateProductCommandValidator();
        var behavior = new ValidationBehavior<CreateProductCommand, Guid>(new[] { validator });

        var invalidCommand = new CreateProductCommand(
            Sku: "a", // muy corto y minúsculas
            Name: "",  // vacío
            Description: null,
            Price: -5m, // negativo
            InitialStock: -2); // negativo

        // Act
        Func<Task> act = async () => await behavior.Handle(
            invalidCommand,
            () => Task.FromResult(Guid.NewGuid()),
            CancellationToken.None);

        // Assert
        var exception = await act.Should().ThrowAsync<ValidationException>();
        exception.Which.Errors.Should().Contain(e => e.PropertyName == "Sku");
        exception.Which.Errors.Should().Contain(e => e.PropertyName == "Name");
        exception.Which.Errors.Should().Contain(e => e.PropertyName == "Price");
        exception.Which.Errors.Should().Contain(e => e.PropertyName == "InitialStock");
    }
}
