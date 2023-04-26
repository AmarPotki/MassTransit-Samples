namespace Saga.Contracts;

public record OrderStatus(Guid OrderId, string CustomerNumber, string PaymentCardNumber, string State);