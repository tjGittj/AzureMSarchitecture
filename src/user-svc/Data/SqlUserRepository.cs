
using Common.Contracts;
using Microsoft.Data.SqlClient;

namespace UserService.Data;
public class SqlUserRepository : IUserRepository
{
    private readonly string _conn;
    public SqlUserRepository(IConfiguration cfg) => _conn = cfg["SQL_CONN_STRING"] ?? "";

    public async Task<UserDto?> GetAsync(string id, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(_conn)) return null;
        using var con = new SqlConnection(_conn);
        await con.OpenAsync(ct);
        var cmd = new SqlCommand("SELECT Id, Email, FullName FROM Users WHERE Id=@id", con);
        cmd.Parameters.AddWithValue("@id", id);
        using var r = await cmd.ExecuteReaderAsync(ct);
        if (!await r.ReadAsync(ct)) return null;
        return new UserDto(r.GetString(0), r.GetString(1), r.GetString(2));
    }

    public async Task<UserDto> CreateAsync(CreateUserRequest req, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(_conn)) throw new InvalidOperationException("SQL_CONN_STRING not set");
        using var con = new SqlConnection(_conn);
        await con.OpenAsync(ct);
        var id = Guid.NewGuid().ToString("n");
        var cmd = new SqlCommand("INSERT INTO Users(Id, Email, FullName) VALUES(@id,@e,@f)", con);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.Parameters.AddWithValue("@e", req.Email);
        cmd.Parameters.AddWithValue("@f", req.FullName);
        await cmd.ExecuteNonQueryAsync(ct);
        return new UserDto(id, req.Email, req.FullName);
    }
}
