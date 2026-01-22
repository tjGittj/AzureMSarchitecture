
using Common.Contracts;
using Common.Messaging;
using Common.Observability;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer().AddSwaggerGen();
builder.Services.AddHealthChecks();
builder.Services.AddDefaultTelemetry("notify-svc", builder.Configuration);

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
    cfg["SERVICEBUS_SUBSCRIPTION"] ?? "notify-svc",
    async (ev, ct) =>
    {
        Console.WriteLine($"[notify] sending confirmation for order {ev.OrderId} to user {ev.UserId}");
        await Task.CompletedTask;
    });

app.MapGet("/", () => Results.Ok("notify-svc ok"));
app.Run();
