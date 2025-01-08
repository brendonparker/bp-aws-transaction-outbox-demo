namespace TransactionalOutboxPatternApp.Domain;

public class OrderStatusChanged : IIntegrationEvent
{
    private readonly Order? _order;
    private readonly long _orderId;
    private readonly string _status = null!;

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
}