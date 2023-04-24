namespace Saga.Contracts;

public record OrderSubmissionAccepted(Guid OrderId, DateTime Timestamp, string Result);
public record OrderSubmissionRejected(Guid OrderId, DateTime Timestamp, string Result);
