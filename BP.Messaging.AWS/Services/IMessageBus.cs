namespace BP.Messaging.AWS.Services;

public interface IMessageBus
{
    Task PublishAsync(MessageEnvelope messageEnvelope, CancellationToken ct = default);
}