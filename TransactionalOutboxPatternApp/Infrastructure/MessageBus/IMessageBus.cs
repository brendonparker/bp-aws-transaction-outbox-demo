using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Options;

namespace TransactionalOutboxPatternApp.Infrastructure.MessageBus;

public interface IMessageBus
{
    Task PublishAsync<T>(string messageGroupId, T message, CancellationToken ct = default);
}

public class MessageBus(IAmazonSQS sqs, IOptions<MessageBusOptions> opts) : IMessageBus
{
    public async Task PublishAsync<TMessage>(string messageGroupId, TMessage message, CancellationToken ct = default)
    {
        var queueUrl = opts.Value.GetQueueUrl<TMessage>();

        await sqs.SendMessageAsync(new SendMessageRequest
        {
            QueueUrl = queueUrl,
            MessageGroupId = messageGroupId,
            MessageBody = MessageEnvelope.CreateJson(message),
        }, ct);
    }
}