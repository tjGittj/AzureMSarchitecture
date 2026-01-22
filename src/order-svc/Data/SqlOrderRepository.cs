
using Common.Contracts;
using Microsoft.Data.SqlClient;

namespace OrderService.Data;
public class SqlOrderRepository : IOrderRepository
{
    private readonly string _conn;
    public SqlOrderRepository(IConfiguration cfg) => _conn = cfg["SQL_CONN_STRING"] ?? "";

    public async Task CreateAsync(OrderCreateRequest req, CancellationToken ct)
    {
        using var con = new SqlConnection(_conn);
        await con.OpenAsync(ct);
        var cmd = new SqlCommand("INSERT INTO Orders(OrderId, UserId, Sku, Quantity, Amount, CreatedAt) VALUES(@o,@u,@s,@q,@a, SYSUTCDATETIME())", con);
        cmd.Parameters.AddWithValue("@o", req.OrderId);
        cmd.Parameters.AddWithValue("@u", req.UserId);
        cmd.Parameters.AddWithValue("@s", req.Sku);
        cmd.Parameters.AddWithValue("@q", req.Quantity);
        cmd.Parameters.AddWithValue("@a", req.Amount);
        await cmd.ExecuteNonQueryAsync(ct);
    }
}
