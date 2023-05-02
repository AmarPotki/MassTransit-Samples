using Automatonymous;
using MassTransit;
using Microsoft.Extensions.Logging;
using Saga.Contracts;

namespace Saga.Components.StateMachines.Activities;

public class AcceptOrderActivity : IStateMachineActivity<OrderState, OrderAccepted>
{
    private readonly ILogger _logger;
    public AcceptOrderActivity(ILogger<AcceptOrderActivity> logger)
    {
        _logger = logger;
    }
    public void Probe(ProbeContext context)
    {
        context.CreateScope("accept-order");
    }

    public void Accept(StateMachineVisitor visitor)
    {
        visitor.Visit(this);
    }

    public async Task Execute(BehaviorContext<OrderState, OrderAccepted> context, IBehavior<OrderState, OrderAccepted> next)
    {
        LogContext.Trace?.Log($"AcceptOrderActivity is Executed: {context.Message.CustomerNumber}");
        _logger.LogTrace($"AcceptOrderActivity is Executed: {context.Message.CustomerNumber}");
       await next.Execute(context);
    }

    public Task Faulted<TException>(BehaviorExceptionContext<OrderState, OrderAccepted, TException> context, IBehavior<OrderState, OrderAccepted> next) where TException : Exception
    {
        return next.Faulted(context);
    }
}