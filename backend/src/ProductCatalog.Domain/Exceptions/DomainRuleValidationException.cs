namespace ProductCatalog.Domain.Exceptions;

/// <summary>
/// Excepción arrojada cuando se viola cualquier invariante o regla de negocio del modelo de dominio.
/// </summary>
public class DomainRuleValidationException : Exception
{
    public DomainRuleValidationException(string message) : base(message)
    {
    }

    public DomainRuleValidationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
