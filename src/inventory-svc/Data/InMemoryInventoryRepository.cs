
namespace InventoryService.Data;
public class InMemoryInventoryRepository : IInventoryRepository
{
    public Task<bool> ReserveAsync(string sku, int quantity, CancellationToken ct) => Task.FromResult(true);
}
