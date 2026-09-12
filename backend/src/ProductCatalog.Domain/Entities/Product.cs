using ProductCatalog.Domain.Exceptions;

namespace ProductCatalog.Domain.Entities;

/// <summary>
/// Entidad raíz de agregado que representa un producto del catálogo con su inventario consolidado.
/// Implementa un Modelo de Dominio Rico (Rich Domain Model) con estado estrictamente encapsulado.
/// </summary>
public class Product
{
    public Guid Id { get; private set; }
    public string Sku { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public decimal Price { get; private set; }
    public int Stock { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    /// <summary>
    /// Constructor protegido sin parámetros requerido exclusivamente por Entity Framework Core.
    /// </summary>
    protected Product()
    {
    }

    /// <summary>
    /// Constructor privado para inicialización validada invoked únicamente por la fábrica Create.
    /// </summary>
    private Product(Guid id, string sku, string name, string? description, decimal price, int initialStock, DateTime createdAtUtc)
    {
        Id = id;
        Sku = sku;
        Name = name;
        Description = description;
        Price = price;
        Stock = initialStock;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = null;
    }

    /// <summary>
    /// Fábrica estática para la creación controlada de nuevos productos validando invariantes.
    /// </summary>
    public static Product Create(
        string sku,
        string name,
        string? description,
        decimal price,
        int initialStock,
        Guid? id = null)
    {
        if (string.IsNullOrWhiteSpace(sku))
        {
            throw new DomainRuleValidationException("El código SKU del producto es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainRuleValidationException("El nombre del producto es obligatorio.");
        }

        if (price <= 0.00m)
        {
            throw new DomainRuleValidationException("El precio unitario debe ser estrictamente mayor a cero.");
        }

        if (initialStock < 0)
        {
            throw new DomainRuleValidationException("El stock inicial no puede ser negativo.");
        }

        return new Product(
            id: id ?? Guid.NewGuid(),
            sku: sku.Trim().ToUpperInvariant(),
            name: name.Trim(),
            description: string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            price: price,
            initialStock: initialStock,
            createdAtUtc: DateTime.UtcNow);
    }

    /// <summary>
    /// Ajusta el stock físico disponible en inventario mediante un incremento (>0) o decremento (<0).
    /// Protege la invariante de no negatividad de stock.
    /// </summary>
    /// <param name="delta">Variación de unidades en inventario.</param>
    /// <returns>Cantidad de stock consolidado resultante.</returns>
    /// <exception cref="DomainRuleValidationException">Si el ajuste ocasionaría un inventario negativo.</exception>
    public int AdjustStock(int delta)
    {
        int resultingStock = Stock + delta;

        if (resultingStock < 0)
        {
            throw new DomainRuleValidationException(
                $"Operación inválida: El ajuste de stock ({delta}) dejaría el stock en negativo ({resultingStock}). Stock actual: {Stock}.");
        }

        Stock = resultingStock;
        UpdatedAtUtc = DateTime.UtcNow;

        return Stock;
    }
}
