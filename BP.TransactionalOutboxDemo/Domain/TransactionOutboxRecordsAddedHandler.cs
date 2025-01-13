using System.Text.Json;
using BP.Messaging.AWS;
using BP.Messaging.AWS.Services;
using BP.TransactionalOutboxDemo.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BP.TransactionalOutboxDemo.Domain;

public class TransactionOutboxRecordsAddedHandler(
    ILogger<TransactionOutboxRecordsAddedHandler> log,
    ApplicationDbContext dbContext,
    IMessageBus messageBus) : IHandler<TransactionOutboxRecordsAdded>
{
    private const int MaxDequeue = 50;

    public async Task<bool> HandleAsync(JsonElement message, CancellationToken cancellationToken)
    {
        var obj = message.Deserialize<TransactionOutboxRecordsAdded>();
        return await HandleAsync(obj!, cancellationToken);
    }

    public async Task<bool> HandleAsync(TransactionOutboxRecordsAdded message, CancellationToken ct = default)
    {
        var records = await DequeueTransactionOutboxRecordsAsync(ct);

        if (records.Count == MaxDequeue)
        {
            log.LogWarning("Somehow we're backed up... Only processing the first maxDequeue records.");
        }

        foreach (var record in records)
        {
            log.LogInformation("Processing Message: {MessageId} {EventType}", record.Id, record.EventType);

            if (!await IncrementAttemptCount(record, ct))
            {
                // Someone else attempted this while we were looking at it.
                // Shouldn't really ever happen in a FIFO queue
                log.LogWarning("Someone else attempted processing this while we were looking at it. Won't try again.");
                continue;
            }

            try
            {
                await messageBus.PublishAsync("TxOutbox", new MessageEnvelope
                {
                    Type = record.EventType,
                    Payload = JsonSerializer.Deserialize<JsonElement>(record.JsonContent),
                }, ct);
            }
            catch (Exception e)
            {
                log.LogWarning(e, "Failed during message publishing...");
                await MarkAsErrored(record, e.Message, ct);
            }

            await MarkAsProcessed(record, ct);
        }

        return true;
    }

    private async Task<List<TransactionOutbox>> DequeueTransactionOutboxRecordsAsync(CancellationToken ct) =>
        // TODO: Add index on is_processed for added perf
        await dbContext.TransactionOutbox
            .Where(x => x.IsProcessed == false)
            .OrderBy(x => x.Id)
            .Take(MaxDequeue)
            .ToListAsync(cancellationToken: ct);

    private async Task MarkAsErrored(TransactionOutbox record, string errorMessage, CancellationToken ct) =>
        await dbContext.TransactionOutbox
            .Where(x => x.Id == record.Id)
            .ExecuteUpdateAsync(entity =>
                    entity
                        .SetProperty(x => x.ErrorMessage, errorMessage),
                cancellationToken: ct);

    private async Task<bool> IncrementAttemptCount(TransactionOutbox record, CancellationToken ct)
    {
        var affectedRows = await dbContext.TransactionOutbox
            .Where(x => x.Id == record.Id)
            .Where(x => x.AttemptCount == record.AttemptCount)
            .ExecuteUpdateAsync(entity =>
                    entity
                        .SetProperty(x => x.AttemptCount, x => record.AttemptCount + 1),
                cancellationToken: ct);
        if (affectedRows == 0) return false;
        record.AttemptCount++;
        return true;
    }

    private async Task MarkAsProcessed(TransactionOutbox record, CancellationToken ct) =>
        await dbContext.TransactionOutbox
            .Where(x => x.Id == record.Id)
            .ExecuteUpdateAsync(entity =>
                    entity
                        .SetProperty(x => x.IsProcessed, true)
                        .SetProperty(x => x.ProcessedAt, DateTime.UtcNow),
                cancellationToken: ct);
}