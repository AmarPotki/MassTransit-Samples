using MassTransit;
using Saga.Contracts;

namespace Saga.Components.StateMachines;

public class OrderStateMachine : MassTransitStateMachine<OrderState>
{

    public OrderStateMachine()
    {
        Event(() => OrderSubmitted, m
            => m.CorrelateById(c => c.Message.OrderId));
        Event(() => CheckOrderEvent, m
            =>
        {
            m.CorrelateById(c => c.Message.OrderId);
            m.OnMissingInstance(c => c.ExecuteAsync(async context =>
            {
                await context.RespondAsync<OrderNotFound>(new OrderNotFound(context.Message.OrderId));
            }));


            Event(() => ExceptionEvent, m
                => m.CorrelateById(c => c.Message.OrderId));
        });

        Event(() => ExceptionEventFaulted, x =>
        {
            x
                .CorrelateById(m => m.Message.Message.OrderId) // Fault<T> includes the original message
                .SelectId(m => m.Message.Message.OrderId);
        });
 



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

        DuringAny(When(CheckOrderEvent).Then(context =>
        {
            //update a field so called update time 
            LogContext.Info?.Log($"Checked: {context.Message.OrderId}");
            context.RespondAsync<OrderStatus>(new(context.Message.OrderId, context.Saga.CustomerNumber, context.Saga.PaymentCardNumber, context.Saga.CurrentState));
        }));
        DuringAny(When(ExceptionEvent).Then(context =>
        {
            //update a field so called update time 
            LogContext.Info?.Log($"Exception Event receive: {context.Message.OrderId}");
            throw new Exception("ExceptionEvent");
        }));

        DuringAny(When(ExceptionEventFaulted).Then(context =>
        {
            //update a field so called update time 
            LogContext.Info?.Log($"ExceptionEventFaulted receive: {context.Message.Message.OrderId}");
          
        }));


    }
    public State Submitted { get; set; }
    public Event<OrderSubmitted> OrderSubmitted { get; set; }
    public Event<CheckOrder> CheckOrderEvent { get; set; }
    public Event<ExceptionEvent> ExceptionEvent { get; set; }

    public Event<Fault<ExceptionEvent>> ExceptionEventFaulted { get; private set; }
}

