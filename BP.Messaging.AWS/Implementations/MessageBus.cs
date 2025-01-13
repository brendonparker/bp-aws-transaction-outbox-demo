using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using BP.Messaging.AWS.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BP.Messaging.AWS.Implementations;

internal class MessageBus(
    ILogger<MessageBus> log,
    IAmazonSQS sqs,
    IOptions<MessageBusOptions> opts) : IMessageBus
{
    public async Task PublishAsync(
        MessageEnvelope message,
        CancellationToken ct = default)
    {
        var queueUrl = opts.Value.GetQueueUrl(message.Type);

        // TODO: Maybe look at "queueUrl" to determine if it should go to SNS or SQS (for fan-out use cases).

        if (queueUrl == null)
        {
            log.LogWarning("No queue url setup for message type: {EventType}. Not publishing anything.", message.Type);
            return;
        }

        await sqs.SendMessageAsync(new SendMessageRequest
        {
            QueueUrl = queueUrl,
            MessageGroupId = queueUrl.EndsWith(".fifo") ? message.MessageGroupId : null,
            MessageBody = JsonSerializer.Serialize(message),
        }, ct);
    }
}