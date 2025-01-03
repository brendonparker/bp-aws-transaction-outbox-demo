namespace TransactionalOutboxPatternApp.Domain;

public class OrderStatusChanged
{
    public required long OrderId { get; set; }
    public required string Status { get; set; }
}