using FluentValidation;

namespace ProductCatalog.Application.Products.Queries.GetProductById;

/// <summary>
/// Validador de GetProductByIdQuery con FluentValidation.
/// </summary>
public class GetProductByIdQueryValidator : AbstractValidator<GetProductByIdQuery>
{
    public GetProductByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El identificador del producto es requerido.");
    }
}
