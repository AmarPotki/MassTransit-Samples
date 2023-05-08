namespace Saga.Contracts;

public class OrderDelayed2
{
    public OrderDelayed2()
    {
        
    }
    public string CustomerNumber { get; set; }
    public Guid OrderId { get; set; }
    public DateTime Timestamp { get; set; }
    public string PaymentCardNumber { get; set; }
    public DateTime DeliveryTime { get; set; }

    public OrderDelayed2(Guid orderId, DateTime timestamp, string customerNumber, string paymentCardNumber,
        DateTime deliveryTime)
    {
        CustomerNumber = customerNumber;
        OrderId = orderId;
        Timestamp = timestamp;
        PaymentCardNumber = paymentCardNumber;
        DeliveryTime = deliveryTime;
    }
}