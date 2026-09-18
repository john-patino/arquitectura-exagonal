# Plataforma de Catálogo e Inventario de Productos
## Guía Académica Integral de Arquitectura Hexagonal (Puertos y Adaptadores)

> **Universidad Popular del Cesar (Unicesar)**  
> **Facultad de Ingenierías y Tecnologías — Ingeniería de Sistemas**  
> **Cátedra de Arquitectura de Software**  
> **Stack:** C# 12 / .NET 8, ASP.NET Core Web API, EF Core 8, PostgreSQL 16, React 18 / TypeScript  

---

## Tabla de Contenido
1. [Introducción y Motivación](#1-introducción-y-motivación)
2. [Fundamentos Teóricos: ¿Qué es la Arquitectura Hexagonal?](#2-fundamentos-teóricos-qué-es-la-arquitectura-hexagonal)
   - [Origen Histórico (Alistair Cockburn, 2005)](#origen-histórico-alistair-cockburn-2005)
   - [La Metáfora del Hexágono](#la-metáfora-del-hexágono)
   - [Superando los Problemas de la Arquitectura en Capas Tradicional](#superando-los-problemas-de-la-arquitectura-en-capas-tradicional)
3. [Anatomía del Patrón: Conceptos Clave](#3-anatomía-del-patrón-conceptos-clave)
   - [El Hexágono Interior: Núcleo de Dominio y Aplicación](#el-hexágono-interior-núcleo-de-dominio-y-aplicación)
   - [Puertos (Ports): Primarios (Driving) vs Secundarios (Driven)](#puertos-ports-primarios-driving-vs-secundarios-driven)
   - [Adaptadores (Adapters): Primarios (Driving) vs Secundarios (Driven)](#adaptadores-adapters-primarios-driving-vs-secundarios-driven)
   - [El Principio de Inversión de Dependencias (DIP)](#el-principio-de-inversión-de-dependencias-dip)
4. [Mapeo de la Arquitectura en la Solución .NET 8](#4-mapeo-de-la-arquitectura-en-la-solución-net-8)
   - [Estructura de Proyectos Físicos (.csproj)](#estructura-de-proyectos-físicos-csproj)
   - [Hexágono Interior: ProductCatalog.Domain](#hexágono-interior-productcatalogdomain)
   - [Hexágono Interior: ProductCatalog.Application](#hexágono-interior-productcatalogapplication)
   - [Hexágono Exterior: ProductCatalog.WebApi (Adaptador Primario)](#hexágono-exterior-productcatalogwebapi-adaptador-primario)
   - [Hexágono Exterior: ProductCatalog.Infrastructure (Adaptador Secundario)](#hexágono-exterior-productcataloginfrastructure-adaptador-secundario)
5. [Traza de Ejecución Paso a Paso: Flujo de una Petición](#5-traza-de-ejecución-paso-a-paso-flujo-de-una-petición)
6. [Diagramas de Arquitectura Interactivos (Archify)](#6-diagramas-de-arquitectura-interactivos-archify)
7. [Beneficios Académicos y Profesionales](#7-beneficios-académicos-y-profesionales)
8. [Guía de Puesta en Marcha y Ejecución Local](#8-guía-de-puesta-en-marcha-y-ejecución-local)
9. [Guía Didáctica: Cómo Extender el Sistema](#9-guía-didáctica-cómo-extender-el-sistema)

---

## 1. Introducción y Motivación

En el desarrollo de software empresarial contemporáneo, uno de los desafíos más críticos es **proteger las reglas de negocio frente a la volatilidad de la tecnología**. Los frameworks web cambian, las bibliotecas ORM evolucionan, las bases de datos relacionales pueden migrar a motores NoSQL, y las interfaces gráficas se reemplazan con el tiempo. 

Sin embargo, **el inventario de productos, las reglas de cálculo de existencias y las invariantes de negocio deben permanecer inalterables**, sin verse afectadas por las decisiones tecnológicas externas.

Este repositorio contiene una implementación de referencia orientada al aprendizaje universitario, diseñada para ilustrar de forma tangible, estricta y profesional la **Arquitectura Hexagonal** (también conocida formalmente como **Patrón de Puertos y Adaptadores**) sobre un sistema real de catálogo e inventario.

---

## 2. Fundamentos Teóricos: ¿Qué es la Arquitectura Hexagonal?

### Origen Histórico (Alistair Cockburn, 2005)
La Arquitectura Hexagonal fue formulada originalmente en **2005 por Alistair Cockburn**, uno de los coautores del *Manifiesto Ágil* (Agile Manifesto). Cockburn observó que los sistemas de software sufrían de un grave acoplamiento bidireccional entre la interfaz de usuario, la lógica de negocio y las bases de datos, lo que imposibilitaba probar la aplicación de forma automatizada sin desplegar servidores web o bases de datos reales.

Cockburn enunció el objetivo central de su propuesta de la siguiente manera:
> *"Permitir que una aplicación sea operada de igual forma por usuarios humanos, otros programas, pruebas automatizadas o scripts de consola, y que sea desarrollada y probada de forma aislada respecto de sus eventuales dispositivos y bases de datos en tiempo de ejecución."*

### La Metáfora del Hexágono
¿Por qué Cockburn eligió la figura geométrica de un **hexágono**?

```
                      [ Adaptador Primario: Web API ]
                                     |
                                     v
                        +-------------------------+
                        |      PUERTO ENTRADA     |
     [ Adaptador CLI ]  |  +-------------------+  |  [ Adaptador Test ]
            \           |  |                   |  |           /
             v          |  |      DOMINIO      |  |          v
         +--------------+  |         Y         |  +--------------+
         | PUERTO ENTR. |  |    APLICACIÓN     |  | PUERTO ENTR. |
         +--------------+  |                   |  +--------------+
                        |  +-------------------+  |
                        |      PUERTO SALIDA      |
                        +-------------------------+
                                     |
                                     v
                     [ Adaptador Secundario: EF Core ]
                                     |
                                     v
                              [( PostgreSQL )]
```

1. **Ruptura de la dimensión unidimensional (Arriba/Abajo):** En la arquitectura en capas tradicional existe una noción jerárquica: la UI está "arriba" y la Base de Datos está "abajo". El hexágono destruye esa jerarquía visual y sitúa el negocio en el **centro**.
2. **Múltiples puertos de interacción:** Un hexágono tiene múltiples lados. Cada lado simboliza un canal de comunicación o puerto a través del cual la aplicación interactúa con diferentes tipos de actores externos (un navegador, una cola de mensajes, un motor de persistencia, un servicio de correo o un arnés de pruebas unitarias).
3. **Simetría exterior:** Todos los elementos tecnológicos externos son adaptadores situados alrededor del hexágono. Ninguno tiene privilegios sobre el dominio.

### Superando los Problemas de la Arquitectura en Capas Tradicional

| Problema en Capas Tradicionales | Solución en Arquitectura Hexagonal |
|---|---|
| **Diseño centrado en la base de datos (Database-Driven):** Las tablas de la base de datos definen las clases de datos del negocio. | **Diseño centrado en el dominio (Domain-Driven):** El modelo de dominio se modela a partir del problema de negocio, ignorando por completo la persistencia. |
| **Modelo Anémico (Anemic Domain Model):** Entidades convertidas en simples bolsas de datos con `get; set;` públicos sin lógica. | **Modelo Rico (Rich Domain Model):** Entidades encapsuladas con métodos de negocio que defienden sus propias invariantes. |
| **Falsa Testeabilidad:** Para probar un caso de uso se requiere una base de datos conectada o frameworks pesados de mocking. | **Testeabilidad Pura:** El núcleo se prueba de forma unitaria en milisegundos sustituyendo los puertos de salida por dobles de prueba simples. |
| **Fuga de dependencias:** Anotaciones de ORM (`[Key]`, `[Table]`) o clases de ASP.NET Core contaminan la capa de negocio. | **Regla de Dependencia Estricta:** El núcleo no tiene referencias a ningún paquete externo ni ORM. |

---

## 3. Anatomía del Patrón: Conceptos Clave

La Arquitectura Hexagonal divide formalmente el universo de un sistema en dos mundos: **El Hexágono Interior (Core)** y **El Hexágono Exterior (Adapters)**.

```
+-------------------------------------------------------------------------+
|                          HEXÁGONO EXTERIOR                              |
|                                                                         |
|   +-----------------------+                 +-----------------------+   |
|   |   DRIVING ADAPTERS    |                 |    DRIVEN ADAPTERS    |   |
|   |  (Adaptadores Entrada)|                 |  (Adaptadores Salida) |   |
|   |                       |                 |                       |   |
|   | - ProductsController  |                 | - ProductRepository   |   |
|   | - Swagger UI          |                 | - CleanDbContext      |   |
|   | - Middleware RFC 7807 |                 | - PostgreSQL Npgsql   |   |
|   +-----------+-----------+                 +-----------^-----------+   |
|               |                                         |               |
|===============|=========================================|===============|
|               |          HEXÁGONO INTERIOR              |               |
|               v                                         |               |
|   +-----------------------+                 +-----------+-----------+   |
|   |     DRIVING PORTS     |                 |     DRIVEN PORTS      |   |
|   |   (Puertos Entrada)   |                 |   (Puertos Salida)    |   |
|   |                       |                 |                       |   |
|   | - CreateProductCmd    |                 | - IProductRepository  |   |
|   | - ListProductsQuery   |                 | - IUnitOfWork         |   |
|   | - AdjustStockCmd      |                 +-----------^-----------+   |
|   +-----------+-----------+                             |               |
|               |                                         |               |
|               v                                         |               |
|   +-----------------------------------------------------+-----------+   |
|   |                     APPLICATION & DOMAIN CORE                   |   |
|   |                                                                 |   |
|   | - Casos de uso CQRS (MediatR Handlers)                         |   |
|   | - Entidad rica: Product (con invariantes y private setters)     |   |
|   | - Excepciones: DomainRuleValidationException                   |   |
|   +-----------------------------------------------------------------+   |
+-------------------------------------------------------------------------+
```

### El Hexágono Interior: Núcleo de Dominio y Aplicación
Representa el valor diferencial de la organización.
1. **Domain Core (Dominio):** Contiene las entidades, Value Objects, reglas de negocio puras y excepciones de dominio. Está escrito exclusivamente en el lenguaje base (C# sin dependencias externas).
2. **Application Core (Aplicación):** Contiene la lógica de los casos de uso, la orquestación, las validaciones de entrada y la definición de las interfaces de comunicación (los Puertos).

### Puertos (Ports): Primarios (Driving) vs Secundarios (Driven)
Un **Puerto** es una especificación o contrato abstracto (típicamente una interfaz en C#) que define una frontera de comunicación:

1. **Puertos Primarios / de Entrada / Conductores (Driving / Inbound Ports):**
   - **Propósito:** Definen **qué puede hacer** el sistema. Representan las operaciones que los actores externos pueden solicitar al núcleo.
   - **Quién los usa:** Los adaptadores primarios (ej. controladores web, interfaz de comandos, tests).
   - **Quién los implementa:** El núcleo de la aplicación (los Handlers de casos de uso).
   - **En este proyecto:** `IRequestHandler<CreateProductCommand, Guid>`, `IRequestHandler<ListProductsQuery, IReadOnlyList<ProductDto>>`, etc.

2. **Puertos Secundarios / de Salida / Conducidos (Driven / Outbound Ports):**
   - **Propósito:** Definen **qué necesita el sistema** del exterior para cumplir sus casos de uso (persistencia, envío de correos, pagos, colas de mensajería).
   - **Quién los usa:** El núcleo de la aplicación (los casos de uso llaman al puerto).
   - **Quién los implementa:** Los adaptadores secundarios en la infraestructura física.
   - **En este proyecto:** `IProductRepository` e `IUnitOfWork`.

### Adaptadores (Adapters): Primarios (Driving) vs Secundarios (Driven)
Un **Adaptador** es un componente técnico que traduce datos entre el mundo exterior y los puertos del hexágono:

1. **Adaptadores Primarios / Driving (Inbound Adapters):**
   - Inician la comunicación ("conducen" la aplicación).
   - Reciben estímulos del exterior (ej. una petición HTTP `POST`, un clic de usuario, un mensaje JSON de un broker).
   - Convierten los datos del formato externo (HTTP Request / JSON) en una llamada comprensible por un Puerto de Entrada (un comando C#).
   - **En este proyecto:** `ProductsController` (ASP.NET Core Web API).

2. **Adaptadores Secundarios / Driven (Outbound Adapters):**
   - Reaccionan a las peticiones del núcleo ("son conducidos" por la aplicación).
   - Reciben peticiones abstractas del puerto de salida y las traducen a una tecnología específica (ej. sentencias SQL de PostgreSQL, queries de MongoDB o invocaciones HTTP a una pasarela).
   - **En este proyecto:** `ProductRepository` y `CleanDbContext` usando EF Core 8 y Npgsql.

### El Principio de Inversión de Dependencias (DIP)
El corazón arquitectónico de los Puertos y Adaptadores es el **Principio de Inversión de Dependencias (DIP)** (la letra 'D' de SOLID):

> *Los módulos de alto nivel (el Dominio y la Aplicación) no deben depender de los módulos de bajo nivel (la Base de Datos y la Web API). Ambos deben depender de abstracciones (Puertos).*

```
Enfoque Tradicional (Acoplado):
[ Lógica de Negocio ] --------> [ Repositorio SQL ] --------> [ Base de Datos ]
(La lógica de negocio depende directamente de la tecnología SQL concreta)

Enfoque Hexagonal con DIP:
[ Lógica de Negocio ] --------> [ Puerto: IProductRepository ] (Interfaz en el Core)
                                            ^
                                            | (Implementa)
                                [ Adaptador: ProductRepository ] (En Infraestructura)
```

Al colocar la interfaz `IProductRepository` dentro del proyecto `Application` e implementar dicha interfaz en `Infrastructure`, la flecha de dependencia en tiempo de compilación se **invierte**: la Infraestructura depende de la Aplicación, y la Aplicación jamás sabe qué motor de base de datos se encuentra detrás del puerto.

---

## 4. Mapeo de la Arquitectura en la Solución .NET 8

Para que la Arquitectura Hexagonal no sea una simple convención de carpetas fácilmente vulnerada, en este proyecto se materializa mediante **proyectos físicos C# separados (.csproj)**. El propio compilador de C# (`CSC`) rechaza la compilación si alguien intenta introducir una referencia prohibida hacia capas externas.

```
backend/
├── ProductCatalog.sln
├── src/
│   ├── ProductCatalog.Domain/          <-- [HEXÁGONO INTERIOR: Dominio Puro]
│   │   ├── Entities/
│   │   │   └── Product.cs
│   │   └── Exceptions/
│   │       ├── DomainRuleValidationException.cs
│   │       └── ProductNotFoundException.cs
│   │
│   ├── ProductCatalog.Application/     <-- [HEXÁGONO INTERIOR: Casos de Uso y Puertos]
│   │   ├── Common/
│   │   │   ├── Behaviors/
│   │   │   │   └── ValidationBehavior.cs
│   │   │   └── Interfaces/             <-- [PUERTOS SECUNDARIOS / OUTBOUND]
│   │   │       ├── IProductRepository.cs
│   │   │       └── IUnitOfWork.cs
│   │   └── Products/
│   │       ├── Commands/               <-- [PUERTOS PRIMARIOS / INBOUND]
│   │       │   ├── CreateProductCommand.cs
│   │       │   ├── UpdateProductStockCommand.cs
│   │       │   └── DeleteProductCommand.cs
│   │       ├── Queries/                <-- [PUERTOS PRIMARIOS / INBOUND]
│   │       │   ├── ListProductsQuery.cs
│   │       │   └── GetProductByIdQuery.cs
│   │       └── Dtos/
│   │           ├── ProductDto.cs
│   │           └── StockSummaryDto.cs
│   │
│   ├── ProductCatalog.Infrastructure/  <-- [HEXÁGONO EXTERIOR: Adaptador Secundario]
│   │   └── Persistence/
│   │       ├── CleanDbContext.cs
│   │       ├── Configurations/
│   │       │   └── ProductConfiguration.cs
│   │       └── Repositories/           <-- [IMPLEMENTACIÓN DE PUERTOS DE SALIDA]
│   │           └── ProductRepository.cs
│   │
│   └── ProductCatalog.WebApi/          <-- [HEXÁGONO EXTERIOR: Adaptador Primario]
│       ├── Controllers/                <-- [CONTROLADORES REST]
│       │   └── ProductsController.cs
│       ├── Middleware/                 <-- [TRADUCTOR DE EXCEPCIONES A RFC 7807]
│       │   └── ExceptionHandlingMiddleware.cs
│       └── Program.cs                  <-- [COMPOSICIÓN DE CONTENEDOR IoC]
│
└── tests/
    ├── ProductCatalog.Domain.Tests/          <-- Pruebas de reglas de negocio
    ├── ProductCatalog.Application.Tests/     <-- Pruebas de casos de uso con Mocks
    └── ProductCatalog.Infrastructure.Tests/  <-- Pruebas reales con Testcontainers
```

---

### Hexágono Interior: ProductCatalog.Domain

Este proyecto es el corazón del sistema. Tiene **cero dependencias externas**: no contiene referencias a ASP.NET Core, ni a EF Core, ni a Newtonsoft/System.Text.Json. Solo utiliza los tipos fundamentales de .NET (`Guid`, `string`, `decimal`, `int`, `DateTime`).

#### Código Didáctico: Entidad Rica vs Modelo Anémico

En un modelo anémico tradicional, cualquier capa externa puede mutar el estado de un producto sin control, provocando estados inconsistentes (por ejemplo, existencias negativas). En nuestra arquitectura hexagonal, la entidad `Product` encapsula su estado protegiendo sus invariantes:

```csharp
namespace ProductCatalog.Domain.Entities;

public class Product
{
    // Setters privados: Nadie fuera de la entidad puede modificar el estado directamente
    public Guid Id { get; private set; }
    public string Sku { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public decimal Price { get; private set; }
    public int Stock { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    // Constructor privado requerido para la hidratación interna del ORM
    private Product() { }

    // Método de Fábrica (Factory Method): Única forma válida de instanciar un producto
    public static Product Create(string sku, string name, string? description, decimal price, int initialStock)
    {
        if (string.IsNullOrWhiteSpace(sku))
            throw new DomainRuleValidationException("El código SKU es obligatorio.");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainRuleValidationException("El nombre del producto es obligatorio.");

        if (price <= 0)
            throw new DomainRuleValidationException("El precio debe ser estrictamente mayor a 0.");

        if (initialStock < 0)
            throw new DomainRuleValidationException("El stock inicial no puede ser negativo.");

        return new Product
        {
            Id = Guid.NewGuid(),
            Sku = sku.Trim().ToUpperInvariant(),
            Name = name.Trim(),
            Description = description?.Trim(),
            Price = price,
            Stock = initialStock,
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    // Regla de negocio de dominio: Ajustar el inventario validando invariantes
    public void AdjustStock(int quantityDelta)
    {
        int newStock = Stock + quantityDelta;

        // INVARIANTE INQUEBRANTABLE: Jamás permitir inventario negativo
        if (newStock < 0)
        {
            throw new DomainRuleValidationException(
                $"No es posible reducir {Math.Abs(quantityDelta)} unidades. Stock disponible actual: {Stock}.");
        }

        Stock = newStock;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
```

---

### Hexágono Interior: ProductCatalog.Application

Contiene la lógica de aplicación. Depende **únicamente** de `ProductCatalog.Domain`. Aquí se definen formalmente los **Puertos de Entrada** y los **Puertos de Salida**.

#### 1. Definición de un Puerto Secundario (Driven Port)
Ubicado en `Application.Common.Interfaces`:

```csharp
namespace ProductCatalog.Application.Common.Interfaces;

// PUERTO SECUNDARIO: La aplicación exige estas capacidades para poder operar
public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> ListAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Product product, CancellationToken cancellationToken = default);
    Task DeleteAsync(Product product, CancellationToken cancellationToken = default);
}

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
```

#### 2. Definición y Manejo de un Puerto Primario (Driving Port)
Los comandos y consultas CQRS representan los Puertos Primarios. El caso de uso orquesta la operación llamando a los puertos de salida:

```csharp
namespace ProductCatalog.Application.Products.Commands;

// CONTRATO DEL PUERTO PRIMARIO (Entrada)
public record CreateProductCommand(
    string Sku,
    string Name,
    string? Description,
    decimal Price,
    int InitialStock
) : IRequest<Guid>;

// MANEJADOR DEL CASO DE USO (Implementación del Puerto Primario en el Core)
public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IProductRepository _productRepository; // Dependencia de Puerto de Salida
    private readonly IUnitOfWork _unitOfWork;               // Dependencia de Puerto de Salida

    // Inyección del contrato abstracto, no de la base de datos física
    public CreateProductCommandHandler(IProductRepository productRepository, IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // 1. Validar unicidad de SKU a través del puerto
        var existing = await _productRepository.GetBySkuAsync(request.Sku, cancellationToken);
        if (existing != null)
            throw new DomainRuleValidationException($"Ya existe un producto registrado con el SKU '{request.Sku}'.");

        // 2. Ejecutar la lógica pura de negocio en la entidad de dominio
        var product = Product.Create(
            request.Sku,
            request.Name,
            request.Description,
            request.Price,
            request.InitialStock
        );

        // 3. Persistir a través del puerto secundario
        await _productRepository.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 4. Retornar el identificador resultante
        return product.Id;
    }
}
```

---

### Hexágono Exterior: ProductCatalog.WebApi (Adaptador Primario)

El controlador HTTP es un **Adaptador de Entrada**. Su única responsabilidad es:
1. Escuchar la solicitud HTTP (`POST /api/v1/products`).
2. Mapear el cuerpo JSON al comando del puerto primario (`CreateProductCommand`).
3. Invocar al mediador (`_mediator.Send`).
4. Retornar la respuesta HTTP adecuada (`201 Created`, `400 Bad Request`, `404 Not Found`).

```csharp
namespace ProductCatalog.WebApi.Controllers;

[ApiController]
[Route("api/v1/products")]
public class ProductsController : ControllerBase
{
    private readonly ISender _mediator;

    public ProductsController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request, CancellationToken ct)
    {
        // Traducción de la petición HTTP externa al puerto de entrada del núcleo
        var command = new CreateProductCommand(
            request.Sku,
            request.Name,
            request.Description,
            request.Price,
            request.InitialStock
        );

        Guid productId = await _mediator.Send(command, ct);

        return CreatedAtAction(nameof(GetById), new { id = productId }, new { id = productId });
    }
}
```

---

### Hexágono Exterior: ProductCatalog.Infrastructure (Adaptador Secundario)

Es un **Adaptador de Salida**. Implementa la interfaz `IProductRepository` usando Entity Framework Core 8 y el proveedor de PostgreSQL `Npgsql`.

```csharp
namespace ProductCatalog.Infrastructure.Persistence.Repositories;

// ADAPTADOR SECUNDARIO: Implementa el contrato del puerto de salida
public class ProductRepository : IProductRepository
{
    private readonly CleanDbContext _context;

    public ProductRepository(CleanDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _context.Products.FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<Product?> GetBySkuAsync(string sku, CancellationToken ct)
    {
        return await _context.Products.FirstOrDefaultAsync(p => p.Sku == sku, ct);
    }

    public async Task AddAsync(Product product, CancellationToken ct)
    {
        await _context.Products.AddAsync(product, ct);
    }

    // ... demás métodos de la interfaz
}
```

---

## 5. Traza de Ejecución Paso a Paso: Flujo de una Petición

A continuación se detalla la secuencia exacta que experimenta una petición de creación de producto (`POST /api/v1/products`):

```
(1) Cliente HTTP / Swagger
        |
        |  POST /api/v1/products { sku: "MON-49", name: "Samsung G9", price: 1299.99, initialStock: 10 }
        v
(2) [ADAPTADOR PRIMARIO] ProductsController (WebApi)
        |
        |  Instancia CreateProductCommand y llama a _mediator.Send(command)
        v
(3) [PUERTO DE ENTRADA / PIPELINE] MediatR Pipeline
        |
        |  Intercepción por ValidationBehavior (FluentValidation)
        |  Valida longitud de campos, rangos y caracteres
        v
(4) [CASO DE USO / APPLICATION CORE] CreateProductCommandHandler
        |
        |  1. Consulta IProductRepository.GetBySkuAsync("MON-49")
        |  2. Invoca Product.Create("MON-49", "Samsung G9", ...)
        v
(5) [DOMINIO PURO] Entidad Product
        |
        |  Valida invariantes de negocio (precio > 0, stock >= 0)
        |  Construye el agregado con private setters y nuevo Guid
        v
(6) [PUERTO DE SALIDA] IProductRepository.AddAsync(product)
        |
        v
(7) [ADAPTADOR SECUNDARIO] ProductRepository (Infrastructure)
        |
        |  Agrega la entidad al ChangeTracker de CleanDbContext
        v
(8) [PUERTO DE SALIDA] IUnitOfWork.SaveChangesAsync()
        |
        v
(9) [BASE DE DATOS] PostgreSQL 16
        |
        |  Ejecuta sentencia SQL: INSERT INTO products (...) VALUES (...)
        |  Confirma la transacción física (COMMIT)
        v
(10) [RETORNO Y CONVERSIÓN HTTP]
        |
        |  Retorna Guid -> ProductsController -> 201 Created (Location: /api/v1/products/{id})
        v
    Cliente recibe confirmación exitosa
```

---

## 6. Diagramas de Arquitectura Interactivos (Archify)

Para facilitar la comprensión espacial y dinámica del sistema a los estudiantes, este proyecto incluye diagramas generados con **Archify**. Estos diagramas están compilados como archivos **HTML autónomos e interactivos**, con soporte para cambio de tema claro/oscuro, paneo, zoom y visualización de trazas animadas.

Los archivos se encuentran en la carpeta `docs/diagrams/`:

### 1. Mapa de Arquitectura y Componentes Hexagonales
- **Archivo:** [`docs/diagrams/hexagonal-architecture.html`](file:///d:/unicesar%202026/arquitectura/demo-arquitectura/arquitectura-exagonal/docs/diagrams/hexagonal-architecture.html)
- **Fuente de especificación:** [`docs/diagrams/hexagonal-architecture.architecture.json`](file:///d:/unicesar%202026/arquitectura/demo-arquitectura/arquitectura-exagonal/docs/diagrams/hexagonal-architecture.architecture.json)
- **Vistas incluidas en el visor interactivo:**
  1. *Flujo de Entrada (Driving / Primario):* Enfoca el cliente, el controlador WebApi, el puerto MediatR y el Dominio.
  2. *Flujo de Salida y Persistencia (Driven):* Aísla la interacción entre el núcleo, el puerto `IProductRepository`, el adaptador EF Core y PostgreSQL.
  3. *Topología Completa del Hexágono:* Muestra la frontera entre el Hexágono Interior (Core) y el Hexágono Exterior (Adaptadores).

### 2. Diagrama de Secuencia y Ciclo de Petición
- **Archivo:** [`docs/diagrams/product-flow.html`](file:///d:/unicesar%202026/arquitectura/demo-arquitectura/arquitectura-exagonal/docs/diagrams/product-flow.html)
- **Fuente de especificación:** [`docs/diagrams/product-flow.sequence.json`](file:///d:/unicesar%202026/arquitectura/demo-arquitectura/arquitectura-exagonal/docs/diagrams/product-flow.sequence.json)
- **Animación Trace:** Permite seguir visualmente el viaje del paquete de datos paso a paso desde el cliente hasta el commit en base de datos.

### 3. Diagrama de Ciclo de Vida del Inventario y Dominio
- **Archivo:** [`docs/diagrams/inventory-lifecycle.html`](file:///d:/unicesar%202026/arquitectura/demo-arquitectura/arquitectura-exagonal/docs/diagrams/inventory-lifecycle.html)
- **Fuente de especificación:** [`docs/diagrams/inventory-lifecycle.lifecycle.json`](file:///d:/unicesar%202026/arquitectura/demo-arquitectura/arquitectura-exagonal/docs/diagrams/inventory-lifecycle.lifecycle.json)
- **Máquina de Estados de Negocio:** Ilustra los estados del agregado `Product` (Registrado, Stock Normal, Ajuste de Stock, Stock Crítico/Bajo, Agotado, Reabastecimiento y Eliminado/Descontinuado), demostrando de forma interactiva por qué el modelo rico defiende sus invariantes ante mutaciones inválidas.

> **Cómo abrirlos:** Puedes abrir directamente los archivos `.html` en cualquier navegador web moderno (Chrome, Firefox, Edge) haciendo doble clic sobre ellos o sirviéndolos localmente.

---

## 7. Beneficios Académicos y Profesionales

1. **Testeabilidad Aislada (Velocidad de Pruebas):**
   - Las pruebas unitarias de `Domain` y `Application` se ejecutan en **milisegundos** porque no levantan ningún servidor HTTP ni conectan con ninguna base de datos.
   - Para probar un caso de uso basta con pasar una implementación simulada (mock) del puerto `IProductRepository` (usando `NSubstitute`).
2. **Independencia Tecnológica y Sustituibilidad:**
   - Si mañana se decide cambiar PostgreSQL por MongoDB, **no se modifica una sola línea de código en `ProductCatalog.Domain` ni en `ProductCatalog.Application`**. Simplemente se crea un nuevo adaptador `MongoProductRepository` en un proyecto de infraestructura diferente.
3. **Simetría y Múltiples Clientes:**
   - Si se requiere agregar un bot de consola, un consumidor de colas RabbitMQ o una interfaz gRPC, se añade como un nuevo **Adaptador Primario**, invocando exactamente los mismos comandos de `Application` sin duplicar lógica.
4. **Resistencia a la Deuda Técnica:**
   - La frontera física impuesta por los proyectos `.csproj` evita que un desarrollador apresurado o inexperto inyecte un `DbContext` directamente dentro de un controlador o una regla de dominio.

---

## 8. Guía de Puesta en Marcha y Ejecución Local

### Prerrequisitos
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (con Docker Compose v2)
- [Node.js v18+](https://nodejs.org/) (opcional, para compilar diagramas de Archify o correr frontend fuera de Docker)

---

### Opción A: Despliegue Completo con Docker Compose (Recomendado)

Levanta la base de datos PostgreSQL, el backend ASP.NET Core y el frontend React con un solo comando:

```bash
docker compose up -d --build
```

#### Servicios Expuestos:
| Servicio | URL Local | Descripción |
|---|---|---|
| **Frontend Web** | `http://localhost:5173` | Aplicación SPA React con catálogo de productos y gestión de stock. |
| **Backend API (Swagger)** | `http://localhost:5000/swagger` | Documentación OpenAPI interactiva de la Web API. |
| **PostgreSQL 16** | `localhost:5432` | Base de datos (`db: product_catalog_db`, `user: clean_user`, `pass: clean_password`). |

---

### Opción B: Ejecución Manual con CLI de .NET

1. **Iniciar la base de datos PostgreSQL en segundo plano:**
   ```bash
   docker compose up -d postgres-db
   ```

2. **Compilar la solución .NET:**
   ```bash
   dotnet build backend/ProductCatalog.sln
   ```

3. **Ejecutar la suite completa de pruebas automatizadas:**
   ```bash
   dotnet test backend/ProductCatalog.sln
   ```

4. **Ejecutar el proyecto WebApi:**
   ```bash
   cd backend/src/ProductCatalog.WebApi
   dotnet run
   ```
   Abre tu navegador en `http://localhost:5000/swagger`.

---

### Pruebas de Endpoints REST con cURL

#### 1. Listar Productos del Catálogo (Seed inicial de 10 productos):
```bash
curl -X GET "http://localhost:5000/api/v1/products" -H "accept: application/json"
```

#### 2. Registrar un Nuevo Producto (Adaptador Primario -> Puerto Primario):
```bash
curl -X POST "http://localhost:5000/api/v1/products" \
  -H "Content-Type: application/json" \
  -d '{
    "sku": "LAP-MACB-M3",
    "name": "Apple MacBook Pro 16 M3 Max",
    "description": "Portátil profesional para desarrollo con 36GB RAM",
    "price": 3499.00,
    "initialStock": 8
  }'
```

#### 3. Ajustar Inventario (Validación de Invariante de Stock):
```bash
curl -X PATCH "http://localhost:5000/api/v1/products/{ID_DEL_PRODUCTO}/stock" \
  -H "Content-Type: application/json" \
  -d '{
    "quantityDelta": -2
  }'
```

*(Si intentas enviar un `quantityDelta` que deje el stock por debajo de 0, el sistema retornará un `400 Bad Request` en formato estandarizado RFC 7807 sin corromper el inventario).*

---

## 9. Guía Didáctica: Cómo Extender el Sistema

Para afianzar los conocimientos adquiridos en la cátedra de Arquitectura de Software, se propone el siguiente ejercicio guiado de extensión:

### Ejercicio: "Notificar por Correo Electrónico cuando el Stock sea Bajo"

Si se solicita enviar un correo cada vez que el stock de un producto quede en menos de 3 unidades, ¿cómo se implementaría respetando la Arquitectura Hexagonal?

1. **Paso 1: Crear el Puerto Secundario (Driven Port)**  
   En `ProductCatalog.Application/Common/Interfaces/`:
   ```csharp
   public interface INotificationService
   {
       Task SendLowStockAlertAsync(string productName, int currentStock, CancellationToken ct);
   }
   ```
   *(Observa que el núcleo solo define la necesidad del negocio, sin saber si se usará SendGrid, SMTP o WhatsApp).*

2. **Paso 2: Invocar el Puerto en el Caso de Uso (Hexágono Interior)**  
   En `UpdateProductStockCommandHandler`, inyectar `INotificationService`. Después de que el producto ejecuta `product.AdjustStock(delta)`, si `product.Stock < 3`, llamar a `_notificationService.SendLowStockAlertAsync(...)`.

3. **Paso 3: Crear el Adaptador Secundario (Hexágono Exterior)**  
   En `ProductCatalog.Infrastructure/Services/`:
   ```csharp
   public class SmtpEmailNotificationAdapter : INotificationService
   {
       public async Task SendLowStockAlertAsync(string productName, int currentStock, CancellationToken ct)
       {
           // Implementación tecnológica con MailKit o System.Net.Mail
           await Task.CompletedTask;
       }
   }
   ```

4. **Paso 4: Registrar en el Contenedor de Inyección de Dependencias**  
   En `ProductCatalog.Infrastructure/DependencyInjection.cs`:
   ```csharp
   services.AddScoped<INotificationService, SmtpEmailNotificationAdapter>();
   ```

**Conclusión del ejercicio:**  
El núcleo de la aplicación nunca se enteró de qué protocolo ni qué biblioteca de correo se utilizó. El sistema se mantuvo 100% desacoplado, testeable y modular.

---

### Preguntas de Autoevaluación para Sustentaciones

1. *¿Por qué `IProductRepository` se encuentra en la capa `Application` y no en la capa `Infrastructure`?*  
   **Respuesta:** Por el Principio de Inversión de Dependencias (DIP). Al ubicar la interfaz en `Application`, el núcleo es el dueño del contrato y la infraestructura se ve obligada a depender de él, no al revés.
2. *¿Cuál es la diferencia exacta entre un Puerto y un Adaptador?*  
   **Respuesta:** Un puerto es una interfaz abstracta que pertenece al núcleo y define un contrato de entrada o salida. Un adaptador es una clase concreta externa que traduce entre una tecnología específica (HTTP, SQL, etc.) y dicho puerto.
3. *¿Por qué la entidad `Product` no tiene setters públicos?*  
   **Respuesta:** Para evitar el antipatrón de modelo anémico y garantizar que ninguna capa externa pueda alterar el estado del producto violando las invariantes de negocio (como tener stock negativo o SKU vacío).

---
*Documentación elaborada para la cátedra de Arquitectura de Software — Universidad Popular del Cesar (Unicesar).*
