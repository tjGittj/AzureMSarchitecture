
using Common.Contracts;
namespace OrderService.Data;
public interface IOrderRepository
{
    Task CreateAsync(OrderCreateRequest req, CancellationToken ct);
}
