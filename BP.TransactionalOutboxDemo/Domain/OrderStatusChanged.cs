namespace BP.TransactionalOutboxDemo.Domain;

public class OrderStatusChanged : IIntegrationEvent, IMessageGroupId
{
    private readonly Order? _order;
    private readonly long _orderId;
    private readonly string _status = null!;
    private readonly string _customerId = null!;

    public OrderStatusChanged()
    {
    }

    public OrderStatusChanged(Order order)
    {
        _order = order;
    }

    public long OrderId
    {
        get => _order?.Id ?? _orderId;
        init => _orderId = value;
    }

    public string Status
    {
        get => _order?.Status ?? _status;
        init => _status = value;
    }

    public string MessageGroupId
    {
        get => _order?.CustomerId ?? _customerId;
        init => _customerId = value;
    }
}