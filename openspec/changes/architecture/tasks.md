## 1. Estructura de la Solución y Configuración Concéntrica (.NET 8)

- [x] 1.1 Crear solución `ProductCatalog.sln` y proyectos `.csproj`: `Domain`, `Application`, `Infrastructure`, `WebApi`
- [x] 1.2 Configurar referencias de proyectos respetando la Regla de Dependencia Concéntrica (WebApi -> Application/Infrastructure -> Domain)
- [x] 1.3 Configurar paquetes NuGet: MediatR, FluentValidation, Npgsql.EntityFrameworkCore.PostgreSQL, Swashbuckle

## 2. Capa de Dominio (Domain)

- [x] 2.1 Implementar entidad rica `Product` con setters privados, fábrica `Create` y método `AdjustStock(delta)`
- [x] 2.2 Implementar excepción de dominio `DomainRuleValidationException`
- [x] 2.3 Implementar excepción de dominio `ProductNotFoundException`

## 3. Capa de Aplicación (Application)

- [x] 3.1 Definir interfaces de persistencia `IProductRepository` e `IUnitOfWork`
- [x] 3.2 Definir DTOs inmutables `ProductDto` y `StockSummaryDto`
- [x] 3.3 Implementar `ValidationBehavior<TRequest, TResponse>` para MediatR con FluentValidation
- [x] 3.4 Implementar `CreateProductCommand`, su validador y handler
- [x] 3.5 Implementar `ListProductsQuery` y handler
- [x] 3.6 Implementar `GetProductByIdQuery`, su validador y handler
- [x] 3.7 Implementar `UpdateProductStockCommand`, su validador y handler
- [x] 3.8 Implementar `DeleteProductCommand`, su validador y handler
- [x] 3.9 Configurar inyección de dependencias en `DependencyInjection.cs` de Application

## 4. Capa de Infraestructura (Infrastructure)

- [x] 4.1 Configurar contexto `CleanDbContext` y mapeo relacional en `ProductConfiguration` (tabla `products`, índice único en `sku`)
- [x] 4.2 Implementar `ProductRepository` y `UnitOfWork`
- [x] 4.3 Configurar semilla de datos (Seed Data) con los 10 productos de muestra
- [x] 4.4 Configurar inyección de dependencias en `DependencyInjection.cs` de Infrastructure

## 5. Capa de Presentación (WebApi)

- [x] 5.1 Implementar `GlobalExceptionHandlerMiddleware` con formato Problem Details (RFC 7807)
- [x] 5.2 Implementar `ProductsController` con endpoints REST (POST, GET, GET /{id}, PATCH /{id}/stock, DELETE /{id})
- [x] 5.3 Configurar CORS para `http://localhost:5173` y Swagger/OpenAPI en `Program.cs`

## 6. Configuración de Despliegue Local (Docker Compose)

- [x] 6.1 Crear `docker-compose.yml` con red `clean-app-net`, volumen `clean_data` y servicio `postgres-db:5432`
- [x] 6.2 Crear `Dockerfile` para `backend-api:5000` con healthcheck conectado a PostgreSQL
- [x] 6.3 Configurar servicio frontend `frontend:5173` con Vite y React 18

## 7. Pruebas Automatizadas y Verificación

- [x] 7.1 Crear proyecto `ProductCatalog.Domain.Tests` y verificar invariantes de `AdjustStock` y fábrica
- [x] 7.2 Crear proyecto `ProductCatalog.Application.Tests` y verificar casos de uso con NSubstitute y validaciones
- [x] 7.3 Crear proyecto `ProductCatalog.Infrastructure.Tests` con Testcontainers.PostgreSql para validar persistencia real
- [x] 7.4 Ejecutar suite completa de pruebas con `dotnet test` y validar ejecución libre de fallos
