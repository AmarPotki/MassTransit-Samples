namespace Saga.Contracts;

public record OrderAccepted(Guid OrderId, DateTime Timestamp, string CustomerNumber, string PaymentCardNumber) ;