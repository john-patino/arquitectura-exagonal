using MediatR;
using ProductCatalog.Application.Products.Dtos;

namespace ProductCatalog.Application.Products.Queries.GetProductById;

/// <summary>
/// Consulta para obtener el detalle de un producto por su identificador GUID.
/// </summary>
public record GetProductByIdQuery(Guid Id) : IRequest<ProductDto>;
