using Microsoft.EntityFrameworkCore;
using ProductCatalog.Application.Common.Interfaces;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementación concreta de IProductRepository sobre Entity Framework Core.
/// </summary>
public class ProductRepository : IProductRepository
{
    private readonly CleanDbContext _context;

    public ProductRepository(CleanDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        var normalizedSku = sku.Trim().ToUpperInvariant();
        return await _context.Products
            .FirstOrDefaultAsync(p => p.Sku == normalizedSku, cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        var list = await _context.Products
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);

        return list.AsReadOnly();
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        await _context.Products.AddAsync(product, cancellationToken);
    }

    public Task DeleteAsync(Product product, CancellationToken cancellationToken = default)
    {
        _context.Products.Remove(product);
        return Task.CompletedTask;
    }
}
