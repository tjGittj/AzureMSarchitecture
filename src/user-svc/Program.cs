
using Common.Contracts;
using Common.Messaging;
using Common.Observability;
using UserService.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer().AddSwaggerGen();
builder.Services.AddHealthChecks();
builder.Services.AddDefaultTelemetry("user-svc", builder.Configuration);

if (!string.IsNullOrWhiteSpace(builder.Configuration["SQL_CONN_STRING"]))
    builder.Services.AddScoped<IUserRepository, SqlUserRepository>();
else
    builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();

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

app.MapGet("/users/{id}", async (string id, IUserRepository repo, CancellationToken ct) =>
{
    var user = await repo.GetAsync(id, ct);
    return user is not null ? Results.Ok(user) : Results.NotFound();
});

app.MapPost("/users", async (CreateUserRequest req, IUserRepository repo, CancellationToken ct) =>
{
    var user = await repo.CreateAsync(req, ct);
    return Results.Created($"/users/{user.Id}", user);
});

app.Run();
