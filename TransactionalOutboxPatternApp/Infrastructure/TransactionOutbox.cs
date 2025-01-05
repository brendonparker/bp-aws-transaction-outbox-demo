namespace TransactionalOutboxPatternApp.Infrastructure;

public class TransactionOutbox
{
    public required long Id { get; set; }
    public required bool IsProcessed { get; set; }
    public required string JsonContent { get; set; }
}