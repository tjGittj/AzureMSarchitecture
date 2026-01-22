
namespace Common.Contracts;

public record OrderCreateRequest(string OrderId, string UserId, string Sku, int Quantity, decimal Amount);
public record OrderCreatedEvent(string OrderId, string UserId, string Sku, int Quantity, decimal Amount, DateTimeOffset CreatedAt);
public record StockReservedEvent(string OrderId, string Sku, int Quantity, bool Success, string? Reason);
public record PaymentProcessedEvent(string OrderId, bool Success, string? Reason);
