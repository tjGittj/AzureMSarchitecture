
using Common.Contracts;
namespace UserService.Data;
public interface IUserRepository
{
    Task<UserDto?> GetAsync(string id, CancellationToken ct);
    Task<UserDto> CreateAsync(CreateUserRequest req, CancellationToken ct);
}
