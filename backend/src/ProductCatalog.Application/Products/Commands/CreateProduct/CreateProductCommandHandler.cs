using MediatR;
using ProductCatalog.Application.Common.Interfaces;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Exceptions;

namespace ProductCatalog.Application.Products.Commands.CreateProduct;

/// <summary>
/// Manejador del caso de uso CreateProductCommand.
/// </summary>
public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductCommandHandler(IProductRepository productRepository, IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var normalizedSku = request.Sku.Trim().ToUpperInvariant();
        var existingProduct = await _productRepository.GetBySkuAsync(normalizedSku, cancellationToken);

        if (existingProduct != null)
        {
            throw new DomainRuleValidationException($"Ya existe un producto registrado con el SKU '{normalizedSku}'.");
        }

        var product = Product.Create(
            sku: normalizedSku,
            name: request.Name,
            description: request.Description,
            price: request.Price,
            initialStock: request.InitialStock);

        await _productRepository.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return product.Id;
    }
}
