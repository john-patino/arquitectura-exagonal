using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Infrastructure.Persistence;
using ProductCatalog.Infrastructure.Persistence.Repositories;
using ProductCatalog.Infrastructure.Persistence.Seed;
using Testcontainers.PostgreSql;
using Xunit;

namespace ProductCatalog.Infrastructure.Tests;

public class ProductRepositoryIntegrationTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("test_catalog_db")
        .WithUsername("test_user")
        .WithPassword("test_password")
        .Build();

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }

    private CleanDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<CleanDbContext>()
            .UseNpgsql(_dbContainer.GetConnectionString())
            .Options;

        var context = new CleanDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    [Fact]
    public async Task ProductRepository_ShouldPersistAndRetrieveProduct_AgainstRealPostgresInstance()
    {
        // Arrange
        using var writeContext = CreateDbContext();
        var repository = new ProductRepository(writeContext);
        var unitOfWork = new UnitOfWork(writeContext);

        var product = Product.Create("SSD-TEST-1TB", "SSD NVMe 1TB", "Prueba", 120m, 15);

        // Act
        await repository.AddAsync(product);
        await unitOfWork.SaveChangesAsync();

        // Assert: Leer en un contexto independiente
        using var readContext = CreateDbContext();
        var readRepository = new ProductRepository(readContext);
        var retrieved = await readRepository.GetByIdAsync(product.Id);

        retrieved.Should().NotBeNull();
        retrieved!.Sku.Should().Be("SSD-TEST-1TB");
        retrieved.Name.Should().Be("SSD NVMe 1TB");
        retrieved.Price.Should().Be(120m);
        retrieved.Stock.Should().Be(15);
    }

    [Fact]
    public async Task ProductRepository_ShouldEnforceSkuUniqueConstraint_AtDatabaseLevel()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new ProductRepository(context);
        var unitOfWork = new UnitOfWork(context);

        var prod1 = Product.Create("SKU-DUPLICATE", "Prod 1", null, 50m, 5);
        var prod2 = Product.Create("SKU-DUPLICATE", "Prod 2", null, 60m, 10);

        await repository.AddAsync(prod1);
        await unitOfWork.SaveChangesAsync();

        // Act
        await repository.AddAsync(prod2);
        Func<Task> act = async () => await unitOfWork.SaveChangesAsync();

        // Assert: Violación de índice único 'ix_products_sku'
        await act.Should().ThrowAsync<DbUpdateException>();
    }

    [Fact]
    public async Task SeedData_ShouldInitializeTenSampleProducts_OnFreshDatabase()
    {
        // Arrange
        using var context = CreateDbContext();

        // Act
        await CatalogDataSeeder.SeedAsync(context);

        // Assert
        var products = await context.Products.ToListAsync();
        products.Should().HaveCount(10);
        products.Should().OnlyContain(p => p.Stock >= 0);
        products.Should().OnlyContain(p => p.Price > 0);
    }
}
