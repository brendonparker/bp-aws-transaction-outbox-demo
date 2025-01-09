namespace BP.AWS.Messaging;

public interface IHandlerInvoker
{
    Task InvokeAsync(MessageEnvelope envelope, CancellationToken ct);
}