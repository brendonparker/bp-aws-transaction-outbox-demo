using BP.Messaging.AWS.Services;

namespace BP.TransactionalOutboxDemo.Domain;

public class OrderStatusChangedHandler(ILogger<OrderStatusChanged> log) : IHandler<OrderStatusChanged>
{
    public async Task<bool> HandleAsync(OrderStatusChanged message, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        log.LogInformation("Handled!");
        return true;
    }
}