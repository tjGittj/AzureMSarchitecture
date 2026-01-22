
using Common.Contracts;
namespace OrderService.Data;
public class InMemoryOrderRepository : IOrderRepository
{
    public Task CreateAsync(OrderCreateRequest req, CancellationToken ct) => Task.CompletedTask;
}
