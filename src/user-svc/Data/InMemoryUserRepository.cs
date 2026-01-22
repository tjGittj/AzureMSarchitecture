
using Common.Contracts;
using System.Collections.Concurrent;

namespace UserService.Data;
public class InMemoryUserRepository : IUserRepository
{
    private readonly ConcurrentDictionary<string, UserDto> _db = new();
    public Task<UserDto?> GetAsync(string id, CancellationToken ct) => Task.FromResult(_db.TryGetValue(id, out var u) ? u : null);
    public Task<UserDto> CreateAsync(CreateUserRequest req, CancellationToken ct)
    {
        var id = Guid.NewGuid().ToString("n");
        var user = new UserDto(id, req.Email, req.FullName);
        _db[id] = user;
        return Task.FromResult(user);
    }
}
