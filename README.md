
# Microservices on Azure App Service (5 services)

- .NET 8 minimal APIs
- Azure Service Bus events (`order.created`)
- Azure SQL for user/order data (in-memory fallback)
- Redis idempotency in payment-svc (optional)
- OpenTelemetry + Swagger + Health

## Local
```bash
docker compose up --build
# visit:
# user:      http://localhost:8081/swagger
# order:     http://localhost:8082/swagger
# inventory: http://localhost:8083/swagger
# payment:   http://localhost:8084/swagger
# notify:    http://localhost:8085/swagger
```

## Environment (App Service)
- `USE_AZURE_SERVICE_BUS=true`
- `SERVICEBUS_CONNECTION_STRING=Endpoint=...`
- `SERVICEBUS_TOPIC=order-events`
- `SERVICEBUS_SUBSCRIPTION=<service-name>`
- `SQL_CONN_STRING=Server=...;Database=...;User Id=...;Password=...;TrustServerCertificate=True;`
- `REDIS_CONNECTION=hostname:6379` (payment-svc only)
- `APPINSIGHTS_CONNECTION_STRING=InstrumentationKey=...` (optional)
