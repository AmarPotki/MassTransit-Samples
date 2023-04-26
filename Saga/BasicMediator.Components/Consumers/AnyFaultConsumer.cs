using MassTransit;

namespace Saga.Components.Consumers;

public class AnyFaultConsumer :
    IConsumer<Fault>
{
    public Task Consume(ConsumeContext<Fault> context)
    {
        LogContext.Info?.Log($"handle Any Fault Event handle: {context.Message.FaultMessageTypes}");
        return Task.CompletedTask;
    }
}