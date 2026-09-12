## Context

El sistema corresponde a una **Plataforma de Catálogo e Inventario de Productos** para Unicesar, estructurado como un monolito desacoplado cliente-servidor (Two-Tier Web Monolith).
El backend se diseña bajo la Arquitectura Limpia (Clean Architecture) de capas concéntricas con estricta regla de dependencias unidireccionales apuntando al núcleo (`Domain`), materializado en proyectos C# físicos separados (.csproj).

## Goals / Non-Goals

**Goals:**
- Implementar cuatro capas concéntricas desacopladas físicamente en .NET 8:
  - `ProductCatalog.Domain`: Entidad rica `Product` con estado encapsulado, constructor de fábrica `Create`, método `AdjustStock(delta)` e invariantes de negocio. Excepciones `DomainRuleValidationException` y `ProductNotFoundException`.
  - `ProductCatalog.Application`: Casos de uso CQRS con MediatR (`CreateProductCommand`, `ListProductsQuery`, `GetProductByIdQuery`, `UpdateProductStockCommand`, `DeleteProductCommand`), validación con FluentValidation e interfaces `IProductRepository` e `IUnitOfWork`.
  - `ProductCatalog.Infrastructure`: Implementación de persistencia con EF Core 8 y PostgreSQL 16 (`CleanDbContext`, `ProductConfiguration`), mapeo de private setters y seed data de 10 productos.
  - `ProductCatalog.WebApi`: Endpoints HTTP con `ProductsController`, Swagger/OpenAPI, CORS para `http://localhost:5173` y middleware global RFC 7807 (Problem Details).
- Topología local contenerizada con Docker Compose en red `clean-app-net` con 3 servicios (`frontend:5173`, `backend-api:5000`, `postgres-db:5432`) y volumen `clean_data`.
- Estrategia de pruebas: unitarias en Domain (invariantes de stock), unitarias en Application (con mocks) e integración en Infrastructure con Testcontainers.PostgreSql.

**Non-Goals:**
- Autenticación o tokens JWT en esta versión (aislado para enfatizar la arquitectura limpia).
- Paginación dinámica en base de datos (se usa el catálogo seed controlado de 10 productos).
- Bus de eventos distribuido asíncrono (RabbitMQ/Kafka); procesamiento síncrono transaccional con `IUnitOfWork`.

## Decisions

### 1. Separación Física por Proyectos (.csproj)
- **Decisión:** Proyectos de biblioteca de clases independientes (`Domain.csproj`, `Application.csproj`, `Infrastructure.csproj`, `WebApi.csproj`).
- **Razón:** El compilador de C# (`CSC`) impone de manera irrompible la regla de dependencias concéntricas. `Domain` no contiene referencia alguna a ninguna otra capa.

### 2. Modelo de Dominio Rico (Rich Domain Model)
- **Decisión:** Todos los setters de la entidad `Product` son privados. Las mutaciones de inventario se restringen a `product.AdjustStock(int delta)`.
- **Razón:** Evita el antipatrón de modelo anémico. Garantiza que bajo ninguna circunstancia el stock pueda alcanzar un valor negativo.

### 3. Pipeline Behaviors para Validación Previa
- **Decisión:** `ValidationBehavior<TRequest, TResponse>` en MediatR ejecutando FluentValidation.
- **Razón:** Valida automáticamente cada comando antes de que alcance el handler, manteniendo los casos de uso limpios y enfocados en orquestación de negocio.

### 4. Persistencia Invertida con EF Core y PostgreSQL
- **Decisión:** Interfaces `IProductRepository` e `IUnitOfWork` residen en `Application` e interactúan con `CleanDbContext` en `Infrastructure`.
- **Razón:** Mantiene el núcleo de la aplicación completamente independiente del motor de base de datos relacional y facilita el mocking en pruebas.

### 5. Contenerización y Entorno Local
- **Decisión:** Docker Compose con red puente `clean-app-net`, healthcheck en `postgres-db` y volumen `clean_data`.
- **Razón:** Garantiza reproducibilidad absoluta del entorno tanto en desarrollo como en pruebas automatizadas.

## Risks / Trade-offs

- **Rigidez en mapeos de EF Core con Private Setters:**
  - *Riesgo:* Necesidad de configuración explícita en EF Core para entidades sin constructores públicos sin argumentos.
  - *Mitigación:* Se define constructor privado/protegido parameterless en `Product` y se usa `builder.Property(p => p.Stock).HasField("_stock")` o acceso directo por propiedad.
- **Dependencia de Docker para Integración:**
  - *Riesgo:* Testcontainers requiere Docker daemon activo para las pruebas de integración.
  - *Mitigación:* Las pruebas unitarias de Domain y Application cubren el 100% de la lógica de negocio sin requerir Docker.
