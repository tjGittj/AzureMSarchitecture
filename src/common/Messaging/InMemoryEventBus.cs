
using System.Collections.Concurrent;

namespace Common.Messaging;

public sealed class InMemoryEventBus : IEventBus
{
    private readonly ConcurrentDictionary<(string Topic, Type Type), List<Func<object, CancellationToken, Task>>> _handlers = new();

    public Task PublishAsync<T>(string topic, T @event, CancellationToken ct = default)
    {
        var key = (topic, typeof(T));
        if (_handlers.TryGetValue(key, out var hs))
        {
            return Task.WhenAll(hs.Select(h => h!(@event!, ct)));
        }
        return Task.CompletedTask;
    }

    public void RegisterHandler<T>(string topic, string subscription, Func<T, CancellationToken, Task> handler)
    {
        var key = (topic, typeof(T));
        _handlers.AddOrUpdate(key,
            _ => new() { (obj, ct) => handler((T)obj, ct) },
            (_, list) => { list.Add((obj, ct) => handler((T)obj, ct)); return list; });
    }
}
