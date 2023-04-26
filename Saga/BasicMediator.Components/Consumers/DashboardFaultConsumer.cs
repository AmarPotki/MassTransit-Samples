using MassTransit;
using Saga.Contracts;

namespace Saga.Components.Consumers;

public class DashboardFaultConsumer :
    IConsumer<Fault<SubmitOrder>>
{
    public DashboardFaultConsumer()
    {
        
    }
    public Task Consume(ConsumeContext<Fault<SubmitOrder>> context)
    {
        LogContext.Info?.Log($"handle SubmitOrder Event handle: {context.Message.Message.OrderId}");
        return Task.CompletedTask;
    }
}