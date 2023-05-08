using MassTransit;

namespace Saga.Components.StateMachines;

public class OrderState :
    SagaStateMachineInstance,ISagaVersion
{
    public string CurrentState { get; set; }

    public string CustomerNumber { get; set; }
    public string PaymentCardNumber { get; set; }

    public string FaultReason { get; set; }
    public Guid CorrelationId { get; set; }
    public int Version { get; set; }
    //for scheduling
    public Guid? HoldDurationToken { get; set; }

}