using Microsoft.EntityFrameworkCore;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Infrastructure.Persistence.Seed;

/// <summary>
/// Semilla de datos para inicializar el catálogo de 10 productos de muestra si la base de datos está vacía.
/// </summary>
public static class CatalogDataSeeder
{
    public static async Task SeedAsync(CleanDbContext context)
    {
        if (await context.Products.AnyAsync())
        {
            return;
        }

        var sampleProducts = new List<Product>
        {
            Product.Create(
                sku: "LAP-DELL-XPS15",
                name: "Portátil Dell XPS 15 9530",
                description: "Intel Core i7 13700H, 32GB RAM DDR5, 1TB NVMe, Pantalla OLED 3.5K",
                price: 2499.99m,
                initialStock: 12,
                id: Guid.Parse("018d9f1a-7b23-71a2-8c11-100000000001")),

            Product.Create(
                sku: "LAP-APPL-MBP16",
                name: "MacBook Pro 16 M3 Max",
                description: "Apple M3 Max 16-Core, 36GB Memoria Unificada, 1TB SSD",
                price: 3499.00m,
                initialStock: 8,
                id: Guid.Parse("018d9f1a-7b23-71a2-8c11-100000000002")),

            Product.Create(
                sku: "MON-LG-34WN80C",
                name: "Monitor UltraWide LG 34 Pulgadas",
                description: "IPS Curvo QHD 3440x1440, USB-C 60W Power Delivery, HDR10",
                price: 549.90m,
                initialStock: 25,
                id: Guid.Parse("018d9f1a-7b23-71a2-8c11-100000000003")),

            Product.Create(
                sku: "MON-DELL-U2723QE",
                name: "Monitor Dell UltraSharp 27 4K",
                description: "IPS Black 4K UHD 3840x2160, Hub USB-C, 98% DCI-P3",
                price: 619.50m,
                initialStock: 15,
                id: Guid.Parse("018d9f1a-7b23-71a2-8c11-100000000004")),

            Product.Create(
                sku: "TEC-LOGI-MXMECH",
                name: "Teclado Mecánico Logitech MX Mechanical",
                description: "Switches Tactile Quiet, Conectividad Bluetooth/Logi Bolt, Iluminación inteligente",
                price: 169.99m,
                initialStock: 40,
                id: Guid.Parse("018d9f1a-7b23-71a2-8c11-100000000005")),

            Product.Create(
                sku: "RAT-LOGI-MXM3S",
                name: "Ratón Inalámbrico Logitech MX Master 3S",
                description: "Sensor 8000 DPI Darkfield, Clics silenciosos, Desplazamiento MagSpeed",
                price: 99.99m,
                initialStock: 50,
                id: Guid.Parse("018d9f1a-7b23-71a2-8c11-100000000006")),

            Product.Create(
                sku: "AUR-SONY-WH1000XM5",
                name: "Auriculares Sony WH-1000XM5",
                description: "Cancelación de ruido Noise Cancelling líder en el mercado, Audio Hi-Res LDAC",
                price: 399.00m,
                initialStock: 18,
                id: Guid.Parse("018d9f1a-7b23-71a2-8c11-100000000007")),

            Product.Create(
                sku: "MEM-CORS-DDR5-32",
                name: "Kit Memoria Corsair Vengeance DDR5 32GB",
                description: "2x16GB 6000MHz CL36 Intel XMP 3.0 / AMD EXPO",
                price: 119.99m,
                initialStock: 30,
                id: Guid.Parse("018d9f1a-7b23-71a2-8c11-100000000008")),

            Product.Create(
                sku: "SSD-SAMS-990PRO-2TB",
                name: "SSD Samsung 990 PRO NVMe M.2 2TB",
                description: "PCIe 4.0 x4, Lectura secuencial hasta 7450 MB/s, Disipador térmico",
                price: 189.90m,
                initialStock: 22,
                id: Guid.Parse("018d9f1a-7b23-71a2-8c11-100000000009")),

            Product.Create(
                sku: "DOCK-CALD-TS4",
                name: "Estación de Acoplamiento CalDigit TS4 Thunderbolt 4",
                description: "18 Puertos, Carga 98W Power Delivery, Soporte Doble Monitor 6K 60Hz",
                price: 399.95m,
                initialStock: 7,
                id: Guid.Parse("018d9f1a-7b23-71a2-8c11-100000000010"))
        };

        await context.Products.AddRangeAsync(sampleProducts);
        await context.SaveChangesAsync();
    }
}
