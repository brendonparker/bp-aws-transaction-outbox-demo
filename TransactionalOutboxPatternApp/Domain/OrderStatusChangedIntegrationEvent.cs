using System.Text.Json;

namespace TransactionalOutboxPatternApp.Domain;

public class OrderStatusChangedIntegrationEvent(Order order) : IIntegrationEvent
{
    public string ToJson() =>
        JsonSerializer.Serialize(new OrderStatusChanged
        {
            OrderId = order.Id,
            Status = order.Status
        });
}