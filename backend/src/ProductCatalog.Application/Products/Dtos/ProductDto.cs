using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Application.Products.Dtos;

/// <summary>
/// Objeto de transferencia de datos inmutable para lectura del catálogo de productos.
/// </summary>
public record ProductDto(
    Guid Id,
    string Sku,
    string Name,
    string? Description,
    decimal Price,
    int Stock,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc)
{
    public static ProductDto FromDomain(Product product) =>
        new(
            product.Id,
            product.Sku,
            product.Name,
            product.Description,
            product.Price,
            product.Stock,
            product.CreatedAtUtc,
            product.UpdatedAtUtc);
}
