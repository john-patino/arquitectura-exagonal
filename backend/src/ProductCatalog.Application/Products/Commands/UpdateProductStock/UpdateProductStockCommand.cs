using MediatR;
using ProductCatalog.Application.Products.Dtos;

namespace ProductCatalog.Application.Products.Commands.UpdateProductStock;

/// <summary>
/// Comando para ajustar el inventario de un producto con un delta (>0 o <0).
/// </summary>
public record UpdateProductStockCommand(Guid Id, int Delta) : IRequest<StockSummaryDto>;
