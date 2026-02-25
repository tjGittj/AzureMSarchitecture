
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080


# Create a non-root user (appuser)
RUN adduser --disabled-password --gecos "" appuser


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY Directory.Build.props ./Directory.Build.props
COPY src/common ./src/common
COPY src/order-svc ./src/order-svc
RUN dotnet restore ./src/order-svc/OrderService.csproj
RUN dotnet publish ./src/order-svc/OrderService.csproj -c Release -o /out

RUN id

FROM base AS final
WORKDIR /app
COPY --from=build /out .


# Switch to non-root user
USER appuser

RUN id

ENTRYPOINT ["dotnet", "OrderService.dll"]
