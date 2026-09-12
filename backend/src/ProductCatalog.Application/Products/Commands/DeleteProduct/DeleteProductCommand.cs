using MediatR;

namespace ProductCatalog.Application.Products.Commands.DeleteProduct;

/// <summary>
/// Comando para eliminar un producto del catálogo por su identificador.
/// </summary>
public record DeleteProductCommand(Guid Id) : IRequest<Unit>;
