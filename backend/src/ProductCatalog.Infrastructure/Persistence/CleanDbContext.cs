using Microsoft.EntityFrameworkCore;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Infrastructure.Persistence;

/// <summary>
/// Contexto de persistencia de Entity Framework Core para la Arquitectura Limpia.
/// </summary>
public class CleanDbContext : DbContext
{
    public CleanDbContext(DbContextOptions<CleanDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CleanDbContext).Assembly);
    }
}
