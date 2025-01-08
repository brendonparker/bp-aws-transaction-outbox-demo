namespace TransactionalOutboxPatternApp.Infrastructure;

public class TransactionOutbox
{
    public required long Id { get; set; }
    public required string EntityType { get; set; }
    public required long EntityId { get; set; }

    public required string EventType { get; set; }
    public required string JsonContent { get; set; }

    public int AttemptCount { get; set; }
    public bool IsProcessed { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
}