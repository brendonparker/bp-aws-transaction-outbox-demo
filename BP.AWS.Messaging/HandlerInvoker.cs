using Microsoft.Extensions.DependencyInjection;

namespace BP.AWS.Messaging;

internal class HandlerInvoker(IServiceProvider serviceProvider) : IHandlerInvoker
{
    public async Task InvokeAsync(MessageEnvelope envelope, CancellationToken ct)
    {
        var handler = serviceProvider.GetRequiredKeyedService<IHandler>(envelope.Type);
        await handler.HandleAsync(envelope.Payload, ct);
    }
}