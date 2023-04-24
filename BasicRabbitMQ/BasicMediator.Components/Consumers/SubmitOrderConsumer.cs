using BasicRabbitMQ.Contracts;
using MassTransit;
using MassTransit.Transports;
using Microsoft.Extensions.Logging;

namespace BasicRabbitMQ.Components.Consumers;

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
    public Task Consume(ConsumeContext<SubmitOrder> context)
    {
        _logger?.Log(LogLevel.Trace, "SubmitOrderConsumer: {CustomerNumber}", context.Message.CustomerNumber);
        if (context.RequestId is null) return Task.CompletedTask;

        if (context.Message.CustomerNumber == "test")
            return context.RespondAsync(new OrderSubmissionRejected(context.Message.OrderId, DateTime.Now, "Reject")); ;
        return context.RespondAsync(new OrderSubmissionAccepted(context.Message.OrderId, DateTime.Now, "Accept"));
    }
}

public class SubmitOrderConsumer2 : IConsumer<SubmitOrder>
{
    private readonly ILogger _logger;

    //public SubmitOrderConsumer()
    //{

    //}
    public SubmitOrderConsumer2(ILogger<SubmitOrderConsumer> logger)
    {
        _logger = logger;
    }
    public Task Consume(ConsumeContext<SubmitOrder> context)
    {
        _logger?.Log(LogLevel.Trace, "SubmitOrderConsumer: {CustomerNumber}", context.Message.CustomerNumber);
        if (context.RequestId is null) return Task.CompletedTask;

        if (context.Message.CustomerNumber == "test")
            return context.RespondAsync(new OrderSubmissionRejected(context.Message.OrderId, DateTime.Now, "Reject")); ;
        return context.RespondAsync(new OrderSubmissionAccepted(context.Message.OrderId, DateTime.Now, "Accept"));
    }
}