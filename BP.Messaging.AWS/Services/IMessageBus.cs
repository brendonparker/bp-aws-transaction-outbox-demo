namespace BP.Messaging.AWS.Services;

public interface IMessageBus
{
    Task PublishAsync(string messageGroupId, MessageEnvelope messageEnvelope, CancellationToken ct = default);
}