namespace Saga.Contracts;

public record OrderDelayed(Guid OrderId, DateTime Timestamp, string CustomerNumber, string PaymentCardNumber, DateTime DeliveryTime) ;