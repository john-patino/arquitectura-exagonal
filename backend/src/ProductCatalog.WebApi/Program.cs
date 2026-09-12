using Microsoft.EntityFrameworkCore;
using ProductCatalog.Application;
using ProductCatalog.Infrastructure;
using ProductCatalog.Infrastructure.Persistence;
using ProductCatalog.Infrastructure.Persistence.Seed;
using ProductCatalog.WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Inyección de dependencias concéntrica (Clean Architecture)
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// Controladores HTTP y JSON Options
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Plataforma de Catálogo e Inventario - Clean Architecture API",
        Version = "v1",
        Description = "API RESTful construida bajo Clean Architecture para la Universidad Popular del Cesar (Unicesar)."
    });
});

// Configuración de CORS para el cliente Vite (React 18)
const string corsPolicyName = "AllowLocalFrontend";
builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicyName, policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://127.0.0.1:5173")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Middleware Global de Excepciones (RFC 7807 Problem Details)
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

// Documentación Swagger/OpenAPI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product Catalog API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseCors(corsPolicyName);

app.MapControllers();

// Inicialización y Seeding de Base de Datos
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var context = services.GetRequiredService<CleanDbContext>();
        if (context.Database.IsRelational())
        {
            await context.Database.EnsureCreatedAsync();
        }
        await CatalogDataSeeder.SeedAsync(context);
        logger.LogInformation("Base de datos verificada y semilla de 10 productos cargada exitosamente.");
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Aviso: No se pudo conectar a PostgreSQL al iniciar (esperado si PostgreSQL aún no está levantado en Docker): {Message}", ex.Message);
    }
}

app.Run();
