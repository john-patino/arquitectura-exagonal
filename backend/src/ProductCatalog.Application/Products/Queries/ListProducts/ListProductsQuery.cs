using MediatR;
using ProductCatalog.Application.Products.Dtos;

namespace ProductCatalog.Application.Products.Queries.ListProducts;

/// <summary>
/// Consulta para listar todos los productos del catálogo.
/// </summary>
public record ListProductsQuery : IRequest<IReadOnlyList<ProductDto>>;
