
using Common.Contracts;
using Common.Messaging;
using Common.Observability;
using OrderService.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer().AddSwaggerGen();
builder.Services.AddHealthChecks();
builder.Services.AddDefaultTelemetry("order-svc", builder.Configuration);

builder.Services.AddSingleton<IEventBus>(sp =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    
    var cs = cfg["SERVICEBUS_CONNECTION_STRING"];
    Console.WriteLine($"RAW=[{cs}] LEN={cs?.Length}");

    return bool.TryParse(cfg["USE_AZURE_SERVICE_BUS"], out var useSb) && useSb
        ? new AzureServiceBusEventBus(cfg["SERVICEBUS_CONNECTION_STRING"]!)
        : new InMemoryEventBus();
});

if (!string.IsNullOrWhiteSpace(builder.Configuration["SQL_CONN_STRING"]))
    builder.Services.AddScoped<IOrderRepository, SqlOrderRepository>();
else
    builder.Services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();

var app = builder.Build();
app.UseSwagger().UseSwaggerUI();
app.MapHealthChecks("/healthz");

app.MapPost("/orders", async (OrderCreateRequest req, IEventBus bus, IOrderRepository repo, IConfiguration cfg, CancellationToken ct) =>
{
    await repo.CreateAsync(req, ct);
    var ev = new OrderCreatedEvent(req.OrderId, req.UserId, req.Sku, req.Quantity, req.Amount, DateTimeOffset.UtcNow);
    await bus.PublishAsync(cfg["SERVICEBUS_TOPIC"] ?? "order-events", ev, ct);
    return Results.Accepted($"/orders/{req.OrderId}", new { req.OrderId });
});


app.MapGet("/sql-ping", async (IConfiguration cfg) =>
{
    var cs = cfg["SQL_CONN_STRING"];
    try
    {
        using var conn = new Microsoft.Data.SqlClient.SqlConnection(cs);
        await conn.OpenAsync();
        return Results.Ok("SQL OK");
    }
    catch (Exception ex)
    {
        // Log the full exception; return a terse message
        Console.Error.WriteLine(ex.ToString());
        return Results.Problem("SQL FAILED: " + ex.Message);
    }
});


app.Run();
