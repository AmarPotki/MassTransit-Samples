namespace BasicMediator.Contracts
{
    public record SubmitOrder(Guid OrderId, DateTime Timestamp, string CustomerNumber, string PaymentCardNumber);

}