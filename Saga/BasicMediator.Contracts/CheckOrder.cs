namespace Saga.Contracts;

public record CheckOrder(Guid OrderId);
public record OrderNotFound(Guid OrderId);
public record ExceptionEvent(Guid OrderId);