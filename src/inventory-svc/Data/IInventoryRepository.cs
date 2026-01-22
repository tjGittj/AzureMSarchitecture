
namespace InventoryService.Data;
public interface IInventoryRepository
{
    Task<bool> ReserveAsync(string sku, int quantity, CancellationToken ct);
}
