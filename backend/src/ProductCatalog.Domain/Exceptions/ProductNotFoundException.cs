namespace ProductCatalog.Domain.Exceptions;

/// <summary>
/// Excepción arrojada cuando se intenta acceder o mutar un producto no existente en el catálogo.
/// </summary>
public class ProductNotFoundException : Exception
{
    public Guid ProductId { get; }

    public ProductNotFoundException(Guid productId)
        : base($"No se encontró ningún producto con el identificador '{productId}'.")
    {
        ProductId = productId;
    }
}
