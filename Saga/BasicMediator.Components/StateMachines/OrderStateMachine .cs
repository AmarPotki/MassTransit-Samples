using MassTransit;
using Saga.Components.StateMachines.Activities;
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



        });
        Event(() => ExceptionEvent, m
            => m.CorrelateById(c => c.Message.OrderId));      
        
        Event(() => OrderDelayedEvent, m
            => m.CorrelateById(c => c.Message.OrderId));

        Event(() => OrderAcceptedEvent, m
            => m.CorrelateById(c => c.Message.OrderId));

        Event(() => ExceptionEventFaulted, x =>
        {
            x
                .CorrelateById(m => m.Message.Message.OrderId) // Fault<T> includes the original message
                .SelectId(m => m.Message.Message.OrderId);
        });
        //does not work in redis, just work database that have support query (sqlserver, mangodb,martin,...)
        Event(() => CustomerAccountClosedEvent, m
            => m.CorrelateBy((saga, context) => saga.CustomerNumber == context.Message.CustomerNumber));



        InstanceState(c => c.CurrentState);
        Initially(When(OrderSubmitted).Then(context =>
        {
            context.Saga.CustomerNumber = context.Message.CustomerNumber;
            context.Saga.PaymentCardNumber = context.Message.PaymentCardNumber;
        }).TransitionTo(Submitted));


        During(Submitted, When(OrderAcceptedEvent)
            .Activity(x =>
            x.OfType<AcceptOrderActivity>()).TransitionTo(Accepted));

        // an option to handle duplicate request
        During(Submitted,
            When(OrderSubmitted)
                .Then(context =>
        {
            LogContext.Info?.Log($"duplicated: {context.Message.CustomerNumber}");
        }), Ignore(OrderSubmitted));

        // other option to handle duplicate request
        //DuringAny(When(OrderSubmitted)
        //    .Then(context =>
        //{
        //    //update a field so called update time 
        //    LogContext.Info?.Log($"duplicated: {context.Message.CustomerNumber}"
        //    );
        //}), Ignore(OrderSubmitted));


        During(Submitted,
            When(CustomerAccountClosedEvent)
                .Then(context =>
                {
                    LogContext.Info?.Log($"cancel: {context.Message.CustomerNumber}");
                }).TransitionTo(Canceled));

        During(Submitted,
            When(OrderDelayedEvent)
                .Then(context =>
                {
                    LogContext.Info?.Log($"Delayed for: {context.Message.DeliveryTime.ToShortDateString()}");
                }).TransitionTo(Delayed));

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
    public State Accepted { get; set; }
    public State Canceled { get; set; }
    public State Delayed { get; set; }
    public Event<OrderSubmitted> OrderSubmitted { get; set; }
    public Event<CheckOrder> CheckOrderEvent { get; set; }
    public Event<OrderAccepted> OrderAcceptedEvent { get; set; }
    public Event<OrderDelayed> OrderDelayedEvent { get; set; }
    public Event<CustomerAccountClosed> CustomerAccountClosedEvent { get; set; }
    public Event<ExceptionEvent> ExceptionEvent { get; set; }
    public Event<Fault<ExceptionEvent>> ExceptionEventFaulted { get; private set; }
}

