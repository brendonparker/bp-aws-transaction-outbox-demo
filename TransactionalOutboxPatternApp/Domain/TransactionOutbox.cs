namespace TransactionalOutboxPatternApp.Domain;

public class TransactionOutbox
{
    public required long Id { get; set; }
    public required bool IsProcessed { get; set; }
    public required string JsonContent { get; set; }
}