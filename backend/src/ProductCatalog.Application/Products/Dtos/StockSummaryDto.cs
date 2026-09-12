namespace ProductCatalog.Application.Products.Dtos;

/// <summary>
/// DTO de respuesta consolidada para ajustes de inventario de un producto.
/// </summary>
public record StockSummaryDto(
    Guid ProductId,
    string Sku,
    int CurrentStock,
    DateTime? UpdatedAtUtc);
