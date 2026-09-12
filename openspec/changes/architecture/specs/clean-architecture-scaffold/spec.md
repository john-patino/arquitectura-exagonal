## ADDED Requirements

### Requirement: Solución Multi-Proyecto en Capas Desacopladas
El sistema SHALL estructurarse como una solución .NET 8 compuesta por cuatro proyectos claramente delimitados según las capas de Clean Architecture: `Domain`, `Application`, `Infrastructure` y `WebApi`.

#### Scenario: Dependencias de Capas Unidireccionales
- **WHEN** se compila la solución .NET
- **THEN** el proyecto `Domain` no tiene referencias a ninguna otra capa, `Application` solo referencia a `Domain`, `Infrastructure` referencia a `Application` y `Domain`, y `WebApi` referencia a `Application` e `Infrastructure` para composición de inyección de dependencias

### Requirement: Dominio Puro y Libre de Frameworks
La capa `Domain` SHALL contener entidades, value objects, interfaces de repositorios y excepciones de negocio sin depender de bibliotecas externas de infraestructura como Entity Framework Core o ASP.NET Core.

#### Scenario: Definición de Entidades e Interfaces
- **WHEN** un desarrollador inspecciona la capa `Domain`
- **THEN** todas las clases de entidades e interfaces de repositorio (`IRepository`) se compilan exclusivamente sobre el BCL estándar de C#/.NET 8

### Requirement: Orquestación y CQRS en la Capa de Aplicación
La capa `Application` SHALL gestionar los casos de uso mediante patrones de comandos y consultas (CQRS) utilizando MediatR, asegurando la validación automática de solicitudes antes de su ejecución.

#### Scenario: Ejecución de Caso de Uso con Validación Exitosa
- **WHEN** la API envía un comando o consulta a través del mediador
- **THEN** el middleware o pipeline de MediatR ejecuta las reglas de FluentValidation y despacha la petición al handler correspondiente

#### Scenario: Falla de Validación en Comando
- **WHEN** un comando con datos inválidos o incompletos es enviado a través del mediador
- **THEN** el pipeline interrumpe la ejecución arrojando una excepción de validación que es capturada por el middleware HTTP

### Requirement: Inversión de Dependencias en Infraestructura
La capa `Infrastructure` SHALL implementar las interfaces de persistencia y servicios externos definidas en `Domain` y `Application`, encapsulando el acceso a base de datos mediante EF Core y/o repositorios en memoria.

#### Scenario: Inyección del Repositorio en Tiempo de Ejecución
- **WHEN** la aplicación inicia y se resuelve el contenedor de dependencias
- **THEN** las implementaciones concretas de infraestructura se inyectan en los handlers de aplicación mediante sus contratos de interfaz

### Requirement: Manejo Centralizado de Excepciones y Respuestas HTTP
La capa `WebApi` SHALL interceptar todas las excepciones no controladas y de dominio mediante un middleware global para transformar las fallas en respuestas normalizadas bajo el estándar Problem Details (RFC 7807).

#### Scenario: Excepción de Negocio no Encontrado
- **WHEN** un caso de uso arroja una excepción de entidad no encontrada
- **THEN** el middleware responde con código de estado HTTP 404 y un payload con formato Problem Details
