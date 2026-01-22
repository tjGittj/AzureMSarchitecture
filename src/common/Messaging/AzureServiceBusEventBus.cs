
using Azure.Messaging.ServiceBus;
using System.Text.Json;

namespace Common.Messaging;

public sealed class AzureServiceBusEventBus : IEventBus, IAsyncDisposable
{
    private readonly ServiceBusClient _client;
    private readonly List<ServiceBusProcessor> _processors = new();
    private readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);
    public AzureServiceBusEventBus(string connectionString) => _client = new(connectionString);

    public async Task PublishAsync<T>(string topic, T @event, CancellationToken ct = default)
    {
        var sender = _client.CreateSender(topic);
        var json = JsonSerializer.Serialize(@event, _json);
        await sender.SendMessageAsync(new ServiceBusMessage(json) { ContentType = "application/json" }, ct);
    }

    public void RegisterHandler<T>(string topic, string subscription, Func<T, CancellationToken, Task> handler)
    {
        var proc = _client.CreateProcessor(topic, subscription, new ServiceBusProcessorOptions { MaxConcurrentCalls = 1, AutoCompleteMessages = false });
        proc.ProcessMessageAsync += async args =>
        {
            try
            {
                var payload = JsonSerializer.Deserialize<T>(args.Message.Body, _json)!;
                await handler(payload, args.CancellationToken);
                await args.CompleteMessageAsync(args.Message);
            }
            catch
            {
                await args.AbandonMessageAsync(args.Message);
            }
        };
        proc.ProcessErrorAsync += args => Task.CompletedTask;
        _processors.Add(proc);
        _ = proc.StartProcessingAsync();
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var p in _processors) await p.DisposeAsync();
        await _client.DisposeAsync();
    }
}
