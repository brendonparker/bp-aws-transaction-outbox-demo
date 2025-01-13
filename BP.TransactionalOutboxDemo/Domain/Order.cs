namespace BP.TransactionalOutboxDemo.Domain;

public class Order : AggregateRoot
{
    public static Order Create()
    {
        Order order = new()
        {
            Id = 0,
            Status = OrderStatus.Created,
        };
        order.AddIntegrationEvent(new OrderStatusChanged(order));
        return order;
    }

    public string CustomerId { get; set; } = "Acme";

    public required string Status { get; set; }

    public void Submit()
    {
        if (Status != OrderStatus.Created)
        {
            // TODO: Don't use exceptions for control flow
            throw new Exception("Order not in proper status.");
        }

        Status = OrderStatus.Submitted;
        AddIntegrationEvent(new OrderStatusChanged(this));
    }
}