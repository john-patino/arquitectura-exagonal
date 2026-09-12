using FluentValidation;

namespace ProductCatalog.Application.Products.Commands.DeleteProduct;

/// <summary>
/// Validador de DeleteProductCommand con FluentValidation.
/// </summary>
public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El identificador del producto es requerido.");
    }
}
