using MediatR;
using ProductCatalog.Application.Common.Interfaces;
using ProductCatalog.Application.Products.Dtos;
using ProductCatalog.Domain.Exceptions;

namespace ProductCatalog.Application.Products.Queries.GetProductById;

/// <summary>
/// Manejador de la consulta GetProductByIdQuery.
/// </summary>
public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    private readonly IProductRepository _productRepository;

    public GetProductByIdQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);

        if (product == null)
        {
            throw new ProductNotFoundException(request.Id);
        }

        return ProductDto.FromDomain(product);
    }
}
