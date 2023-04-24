namespace Saga.Contracts;

public record OrderSubmitted(Guid OrderId, DateTime Timestamp, string CustomerNumber, string PaymentCardNumber) ;