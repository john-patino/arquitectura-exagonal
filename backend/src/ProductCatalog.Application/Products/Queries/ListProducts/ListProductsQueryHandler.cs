using MediatR;
using ProductCatalog.Application.Common.Interfaces;
using ProductCatalog.Application.Products.Dtos;

namespace ProductCatalog.Application.Products.Queries.ListProducts;

/// <summary>
/// Manejador de la consulta ListProductsQuery.
/// </summary>
public class ListProductsQueryHandler : IRequestHandler<ListProductsQuery, IReadOnlyList<ProductDto>>
{
    private readonly IProductRepository _productRepository;

    public ListProductsQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IReadOnlyList<ProductDto>> Handle(ListProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _productRepository.ListAllAsync(cancellationToken);

        return products
            .Select(ProductDto.FromDomain)
            .ToList()
            .AsReadOnly();
    }
}
