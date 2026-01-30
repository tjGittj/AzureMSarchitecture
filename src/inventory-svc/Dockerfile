
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY Directory.Build.props ./Directory.Build.props
COPY src/common ./src/common
COPY src/inventory-svc ./src/inventory-svc
RUN dotnet restore ./src/inventory-svc/InventoryService.csproj
RUN dotnet publish ./src/inventory-svc/InventoryService.csproj -c Release -o /out

FROM base AS final
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["dotnet", "InventoryService.dll"]
