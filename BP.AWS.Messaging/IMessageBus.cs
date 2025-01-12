namespace BP.AWS.Messaging;

public interface IMessageBus
{
    Task PublishAsync(string messageGroupId, MessageEnvelope messageEnvelope, CancellationToken ct = default);
}