namespace TransactionalOutboxPatternApp.Domain;

public class Order : AggregateRoot
{
    public static Order Create()
    {
        Order order = new()
        {
            Id = 0,
            Status = OrderStatus.Created
        };
        order.AddIntegrationEvent(new OrderStatusChanged(order));
        return order;
    }

    public required long Id { get; set; } = 0;
    public required string Status { get; set; }

    public void Submit()
    {
        if (Status != OrderStatus.Created) throw new Exception();
        Status = OrderStatus.Submitted;
        AddIntegrationEvent(new OrderStatusChanged(this));
    }
}