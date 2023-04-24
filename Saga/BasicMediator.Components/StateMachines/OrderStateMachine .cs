using MassTransit;
using Saga.Contracts;

namespace Saga.Components.StateMachines;

public class OrderStateMachine : MassTransitStateMachine<OrderState>
{

    public OrderStateMachine()
    {
        Event(() => OrderSubmitted, m
            => m.CorrelateById(c => c.Message.OrderId));
        InstanceState(c => c.CurrentState);
        Initially(When(OrderSubmitted).TransitionTo(Submitted));

        // an option to handle duplicate request
        During(Submitted, When(OrderSubmitted).Then(context =>
        {
            LogContext.Info?.Log($"duplicated: {context.Message.CustomerNumber}"
            );
        }), Ignore(OrderSubmitted));

        // other option to handle duplicate request
        DuringAny(When(OrderSubmitted)
            .Then(context =>
        {
            //update a field so called update time 
            LogContext.Info?.Log($"duplicated: {context.Message.CustomerNumber}"
            );
        }), Ignore(OrderSubmitted));
    }
    public State Submitted { get; set; }
    public Event<OrderSubmitted> OrderSubmitted { get; set; }
}