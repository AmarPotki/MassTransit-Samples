using BasicMediator.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace BasicMediator.Components.Consumers;

public class SubmitOrderConsumer:IConsumer<SubmitOrder>
{
    private readonly ILogger<SubmitOrderConsumer> _logger;

    public SubmitOrderConsumer(ILogger<SubmitOrderConsumer> logger)
    {
        _logger = logger;
    }
    public  Task Consume(ConsumeContext<SubmitOrder> context)
    {
        _logger?.Log(LogLevel.Debug, "SubmitOrderConsumer: {CustomerNumber}", context.Message.CustomerNumber);

        if (context.Message.CustomerNumber=="test")
            return context.RespondAsync(new OrderSubmissionRejected(context.Message.OrderId, DateTime.Now, "Reject")); ;
        return  context.RespondAsync(new OrderSubmissionAccepted(context.Message.OrderId, DateTime.Now,"Accept"));
    }
}