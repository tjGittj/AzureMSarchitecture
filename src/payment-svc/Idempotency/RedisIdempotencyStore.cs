
using StackExchange.Redis;

namespace PaymentService.Idempotency;

public class RedisIdempotencyStore
{
    private readonly IDatabase? _db;
    public RedisIdempotencyStore(IConfiguration cfg)
    {
        var conn = cfg["REDIS_CONNECTION"];
        _db = string.IsNullOrEmpty(conn) ? null : ConnectionMultiplexer.Connect(conn).GetDatabase();
    }
    public async Task<bool> TryBeginAsync(string key, TimeSpan ttl)
    {
        if (_db is null) return true; // allow when no redis configured
        return await _db.StringSetAsync("idem:" + key, "1", ttl, when: When.NotExists);
    }
}
