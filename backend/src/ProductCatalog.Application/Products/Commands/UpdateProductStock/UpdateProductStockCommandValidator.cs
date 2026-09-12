using FluentValidation;

namespace ProductCatalog.Application.Products.Commands.UpdateProductStock;

/// <summary>
/// Validador para UpdateProductStockCommand con FluentValidation.
/// </summary>
public class UpdateProductStockCommandValidator : AbstractValidator<UpdateProductStockCommand>
{
    public UpdateProductStockCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El identificador del producto es requerido.");

        RuleFor(x => x.Delta)
            .NotEqual(0).WithMessage("El ajuste de stock (Delta) no puede ser cero.");
    }
}
