
using Common.Contracts;
using Common.Messaging;
using Common.Observability;
using InventoryService.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer().AddSwaggerGen();
builder.Services.AddHealthChecks();
builder.Services.AddDefaultTelemetry("inventory-svc", builder.Configuration);

builder.Services.AddSingleton<IInventoryRepository, InMemoryInventoryRepository>();

builder.Services.AddSingleton<IEventBus>(sp =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    return bool.TryParse(cfg["USE_AZURE_SERVICE_BUS"], out var useSb) && useSb
        ? new AzureServiceBusEventBus(cfg["SERVICEBUS_CONNECTION_STRING"]!)
        : new InMemoryEventBus();
});

var app = builder.Build();
app.UseSwagger().UseSwaggerUI();
app.MapHealthChecks("/healthz");

var bus = app.Services.GetRequiredService<IEventBus>();
var cfg = app.Configuration;
bus.RegisterHandler<OrderCreatedEvent>(cfg["SERVICEBUS_TOPIC"] ?? "order-events",
    cfg["SERVICEBUS_SUBSCRIPTION"] ?? "inventory-svc",
    async (ev, ct) =>
    {
        Console.WriteLine($"[inventory] reserving {ev.Quantity} of {ev.Sku} for order {ev.OrderId} -> true");
        await Task.CompletedTask;
    });

app.MapGet("/inventory/{sku}", (string sku) => Results.Ok(new { sku, available = 100 }));
app.Run();
