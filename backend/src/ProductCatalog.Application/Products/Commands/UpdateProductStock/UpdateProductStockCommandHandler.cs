using MediatR;
using ProductCatalog.Application.Common.Interfaces;
using ProductCatalog.Application.Products.Dtos;
using ProductCatalog.Domain.Exceptions;

namespace ProductCatalog.Application.Products.Commands.UpdateProductStock;

/// <summary>
/// Manejador de UpdateProductStockCommand. Invoca el método de dominio rico Product.AdjustStock.
/// </summary>
public class UpdateProductStockCommandHandler : IRequestHandler<UpdateProductStockCommand, StockSummaryDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductStockCommandHandler(IProductRepository productRepository, IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<StockSummaryDto> Handle(UpdateProductStockCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);

        if (product == null)
        {
            throw new ProductNotFoundException(request.Id);
        }

        // Ejecución de la mutación sobre el agregado con verificación de invariantes
        product.AdjustStock(request.Delta);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new StockSummaryDto(
            ProductId: product.Id,
            Sku: product.Sku,
            CurrentStock: product.Stock,
            UpdatedAtUtc: product.UpdatedAtUtc);
    }
}
