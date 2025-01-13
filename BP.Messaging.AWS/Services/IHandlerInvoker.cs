namespace BP.Messaging.AWS.Services;

public interface IHandlerInvoker
{
    Task InvokeAsync(MessageEnvelope envelope, CancellationToken ct);
}