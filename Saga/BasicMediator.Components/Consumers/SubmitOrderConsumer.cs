using MassTransit;
using MassTransit.Transports;
using Microsoft.Extensions.Logging;
using Saga.Contracts;

namespace Saga.Components.Consumers;

public class SubmitOrderConsumer : IConsumer<SubmitOrder>
{
    private readonly ILogger _logger;

    //public SubmitOrderConsumer()
    //{

    //}
    public SubmitOrderConsumer(ILogger<SubmitOrderConsumer> logger)
    {
        _logger = logger;
    }
    public async Task Consume(ConsumeContext<SubmitOrder> context)
    {

        if (context.Message.CustomerNumber == "exception")
        {
            throw new Exception("SubmitOrder exception");
        }

        _logger?.Log(LogLevel.Trace, "SubmitOrderConsumer: {CustomerNumber}", context.Message.CustomerNumber);
        if (context.RequestId is not null)
        {
            if (context.Message.CustomerNumber == "test")
                await context.RespondAsync(new OrderSubmissionRejected(context.Message.OrderId, DateTime.Now, "Reject"));
            await context.RespondAsync(new OrderSubmissionAccepted(context.Message.OrderId, DateTime.Now, "Accept"));

        }

        await context.Publish(new OrderSubmitted(context.Message.OrderId, DateTime.Now, context.Message.CustomerNumber, context.Message.PaymentCardNumber));

    }
}