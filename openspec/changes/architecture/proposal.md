## Why

La implementación de la **Plataforma de Catálogo e Inventario de Productos** bajo los principios de Arquitectura Limpia (Clean Architecture) con C# y .NET 8 desacopla las reglas esenciales del negocio de los detalles tecnológicos (PostgreSQL, EF Core, ASP.NET Core, React). Esto garantiza la integridad estricta del inventario, la testabilidad del dominio y un mantenimiento evolutivo robusto para el sistema de Unicesar.

## What Changes

- Establecimiento de la estructura en cuatro capas concéntricas físicas (`.csproj`):
  - **`Domain`**: Entidad rica `Product` con atributos encapsulados (private setters), método de dominio `AdjustStock(delta)` e invariantes inmutables ante inconsistencias. Excepciones de dominio `DomainRuleValidationException` y `ProductNotFoundException`. Sin dependencias externas.
  - **`Application`**: Orquestación CQRS mediante MediatR, validación automática en pipeline con FluentValidation, contratos de persistencia `IProductRepository` e `IUnitOfWork`, DTOs inmutables (`ProductDto`, `StockSummaryDto`) y casos de uso (`CreateProductCommand`, `ListProductsQuery`, `GetProductByIdQuery`, `UpdateProductStockCommand`, `DeleteProductCommand`).
  - **`Infrastructure`**: Persistencia relacional con PostgreSQL 16 y EF Core (`CleanDbContext`, `ProductConfiguration`), repositorios concretos y semilla de 10 productos de muestra.
  - **`WebApi`**: Host HTTP ASP.NET Core con `ProductsController`, CORS para frontend Vite:5173, middleware global Problem Details (RFC 7807) y Swagger/OpenAPI.
- Especificación de despliegue local contenerizado con Docker Compose (frontend en 5173, backend en 5000, postgres en 5432, red `clean-app-net` y volumen `clean_data`).
- Estrategia de pruebas completa: unitarias en Domain y Application (con mocks) e integración en Infrastructure con Testcontainers.PostgreSql.

## Capabilities

### New Capabilities
- `clean-architecture-scaffold`: Define la taxonomía concéntrica de proyectos, la regla de dependencia estricta hacia el centro, el pipeline de MediatR y el middleware RFC 7807.
- `product-catalog-inventory`: Especifica los requerimientos funcionales de catálogo y control de stock de productos, garantizando la invariante de no negatividad de inventario.

### Modified Capabilities
<!-- No existen especificaciones previas en openspec/specs/. -->

## Impact

- Creación de la solución `.sln` y proyectos .NET 8 (`Domain`, `Application`, `Infrastructure`, `WebApi`, más proyectos de pruebas).
- Definición de especificaciones formales ejecutables en `openspec/project.yaml`, `openspec/layers/`, `openspec/infrastructure/` y `openspec/testing/`.
- Configuración de entorno Docker Compose para ejecución local integrada.
