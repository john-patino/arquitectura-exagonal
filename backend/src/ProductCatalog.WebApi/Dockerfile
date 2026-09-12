# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar archivos de proyectos para aprovechar caché de capas Docker en restore
COPY ProductCatalog.sln ./
COPY src/ProductCatalog.Domain/ProductCatalog.Domain.csproj src/ProductCatalog.Domain/
COPY src/ProductCatalog.Application/ProductCatalog.Application.csproj src/ProductCatalog.Application/
COPY src/ProductCatalog.Infrastructure/ProductCatalog.Infrastructure.csproj src/ProductCatalog.Infrastructure/
COPY src/ProductCatalog.WebApi/ProductCatalog.WebApi.csproj src/ProductCatalog.WebApi/
COPY tests/ProductCatalog.Domain.Tests/ProductCatalog.Domain.Tests.csproj tests/ProductCatalog.Domain.Tests/
COPY tests/ProductCatalog.Application.Tests/ProductCatalog.Application.Tests.csproj tests/ProductCatalog.Application.Tests/
COPY tests/ProductCatalog.Infrastructure.Tests/ProductCatalog.Infrastructure.Tests.csproj tests/ProductCatalog.Infrastructure.Tests/

RUN dotnet restore ProductCatalog.sln

# Copiar todo el código fuente y compilar
COPY src/ src/
COPY tests/ tests/
RUN dotnet publish src/ProductCatalog.WebApi/ProductCatalog.WebApi.csproj -c Release -o /app/publish /p:UseAppHost=false

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ProductCatalog.WebApi.dll"]
