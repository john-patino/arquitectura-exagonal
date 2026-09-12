using FluentValidation;

namespace ProductCatalog.Application.Products.Commands.CreateProduct;

/// <summary>
/// Validador declarativo para CreateProductCommand con FluentValidation.
/// </summary>
public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Sku)
            .NotEmpty().WithMessage("El SKU es requerido.")
            .MaximumLength(50).WithMessage("El SKU no puede exceder 50 caracteres.")
            .Matches("^[A-Z0-9-]+$").WithMessage("El SKU solo puede contener caracteres alfanuméricos en mayúsculas y guiones.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre del producto es requerido.")
            .MaximumLength(150).WithMessage("El nombre no puede exceder 150 caracteres.");

        RuleFor(x => x.Price)
            .GreaterThan(0.00m).WithMessage("El precio debe ser estrictamente mayor a cero.");

        RuleFor(x => x.InitialStock)
            .GreaterThanOrEqualTo(0).WithMessage("El stock inicial no puede ser negativo.");
    }
}
