
namespace Common.Contracts;

public record UserDto(string Id, string Email, string FullName);
public record CreateUserRequest(string Email, string FullName);
