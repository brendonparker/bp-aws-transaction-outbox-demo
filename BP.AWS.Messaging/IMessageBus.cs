namespace BP.AWS.Messaging;

public interface IMessageBus
{
    Task PublishAsync<T>(string messageGroupId, T message, CancellationToken ct = default);
}