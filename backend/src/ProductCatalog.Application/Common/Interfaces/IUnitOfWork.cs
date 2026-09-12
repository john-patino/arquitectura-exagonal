namespace ProductCatalog.Application.Common.Interfaces;

/// <summary>
/// Contrato de Unidad de Trabajo para garantizar transaccionalidad atómica en mutaciones de estado.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
