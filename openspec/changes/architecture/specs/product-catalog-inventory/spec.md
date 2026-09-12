## ADDED Requirements

### Requirement: Creación de Producto en Catálogo
El sistema SHALL permitir registrar un nuevo producto en el catálogo mediante `CreateProductCommand`, asegurando la unicidad del SKU y validando las reglas de precio positivo y stock no negativo.

#### Scenario: Registro Exitoso de Producto
- **WHEN** se envía un comando con SKU único, nombre, precio mayor a cero y stock inicial mayor o igual a cero
- **THEN** el sistema persiste la entidad en PostgreSQL, confirma la transacción con IUnitOfWork y retorna código 201 Created con el GUID del nuevo producto

#### Scenario: Rechazo por SKU Duplicado
- **WHEN** se intenta registrar un producto con un SKU que ya existe en la base de datos
- **THEN** el handler arroja una excepción `DomainRuleValidationException` y la API retorna código HTTP 400 Bad Request

#### Scenario: Rechazo por Validación de Entrada
- **WHEN** se envía un comando con SKU inválido, precio menor o igual a cero, o nombre vacío
- **THEN** el middleware `ValidationBehavior` detiene la ejecución antes del handler y retorna código HTTP 400 Bad Request con los detalles de validación

### Requirement: Consulta de Catálogo y Detalle
El sistema SHALL permitir consultar la lista completa de productos disponibles mediante `ListProductsQuery` y obtener el detalle de un producto individual por su identificador mediante `GetProductByIdQuery`.

#### Scenario: Listado Completo de Productos
- **WHEN** un cliente consulta el catálogo en `/api/v1/products`
- **THEN** el sistema retorna la colección completa de `ProductDto` con código HTTP 200 OK

#### Scenario: Consulta de Producto Existente
- **WHEN** se solicita `/api/v1/products/{id}` con un identificador GUID válido y existente
- **THEN** el sistema retorna el `ProductDto` correspondiente con código HTTP 200 OK

#### Scenario: Consulta de Producto Inexistente
- **WHEN** se solicita `/api/v1/products/{id}` con un GUID que no existe en el catálogo
- **THEN** el sistema arroja `ProductNotFoundException` y la API responde con código HTTP 404 Not Found bajo el formato Problem Details

### Requirement: Ajuste Controlado de Stock de Inventario
El sistema SHALL permitir modificar el stock de un producto mediante `UpdateProductStockCommand` invocando el método de dominio `product.AdjustStock(delta)`, asegurando que el inventario resultante nunca sea menor a cero.

#### Scenario: Ajuste de Stock Válido
- **WHEN** se envía una solicitud PATCH con un delta positivo o negativo que mantiene el stock >= 0
- **THEN** el sistema actualiza el stock, registra la marca de tiempo `UpdatedAtUtc`, persiste los cambios en la base de datos y retorna código HTTP 200 OK con el resumen actualizado

#### Scenario: Rechazo de Ajuste por Stock Insuficiente
- **WHEN** se envía una solicitud PATCH con un delta negativo cuya magnitud supera el stock disponible actual
- **THEN** el método de dominio lanza `DomainRuleValidationException`, la transacción no se persiste y la API retorna código HTTP 400 Bad Request

### Requirement: Eliminación de Producto
El sistema SHALL permitir la eliminación física de un producto del catálogo mediante `DeleteProductCommand`.

#### Scenario: Eliminación Exitosa
- **WHEN** se solicita la eliminación de un producto existente por su GUID
- **THEN** el sistema elimina el registro de PostgreSQL y retorna código HTTP 204 No Content

#### Scenario: Eliminación de Producto Inexistente
- **WHEN** se solicita la eliminación de un producto con un GUID que no existe
- **THEN** el sistema retorna código HTTP 404 Not Found
