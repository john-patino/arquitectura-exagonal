using ProductCatalog.Application.Common.Interfaces;

namespace ProductCatalog.Infrastructure.Persistence;

/// <summary>
/// Implementación concreta de IUnitOfWork que delega en SaveChangesAsync de CleanDbContext.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly CleanDbContext _context;

    public UnitOfWork(CleanDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
