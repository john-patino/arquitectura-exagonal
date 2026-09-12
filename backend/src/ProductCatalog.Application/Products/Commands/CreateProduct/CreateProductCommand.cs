using MediatR;

namespace ProductCatalog.Application.Products.Commands.CreateProduct;

/// <summary>
/// Comando para crear un nuevo producto en el catálogo.
/// </summary>
public record CreateProductCommand(
    string Sku,
    string Name,
    string? Description,
    decimal Price,
    int InitialStock) : IRequest<Guid>;
