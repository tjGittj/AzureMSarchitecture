
using Common.Messaging;
using Common.Observability;
using PaymentService.Idempotency;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer().AddSwaggerGen();
builder.Services.AddHealthChecks();
builder.Services.AddDefaultTelemetry("payment-svc", builder.Configuration);

builder.Services.AddSingleton<IEventBus>(sp =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    return bool.TryParse(cfg["USE_AZURE_SERVICE_BUS"], out var useSb) && useSb
        ? new AzureServiceBusEventBus(cfg["SERVICEBUS_CONNECTION_STRING"]!)
        : new InMemoryEventBus();
});

builder.Services.AddSingleton<RedisIdempotencyStore>();

var app = builder.Build();
app.UseSwagger().UseSwaggerUI();
app.MapHealthChecks("/healthz");

app.MapPost("/payments", async (HttpRequest req, RedisIdempotencyStore idem, CancellationToken ct) =>
{
    var key = req.Headers["Idempotency-Key"].ToString();
    if (string.IsNullOrWhiteSpace(key)) return Results.BadRequest("Missing Idempotency-Key header");
    if (!await idem.TryBeginAsync(key, TimeSpan.FromMinutes(10)))
        return Results.StatusCode(409);

    await Task.Delay(150, ct);
    return Results.Ok(new { status = "paid" });
});

app.Run();
