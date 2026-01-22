
namespace Common.Messaging;

public interface IEventBus
{
    Task PublishAsync<T>(string topic, T @event, CancellationToken ct = default);
    void RegisterHandler<T>(string topic, string subscription, Func<T, CancellationToken, Task> handler);
}
