using Microsoft.Extensions.DependencyInjection;

namespace BP.AWS.Messaging;

internal class HandlerInvoker(IServiceProvider serviceProvider) : IHandlerInvoker
{
    public async Task InvokeAsync(MessageEnvelope messageEnvelope, CancellationToken ct)
    {
        var handler = serviceProvider.GetRequiredKeyedService<HandlerWrapper>(messageEnvelope.Type);
        await handler(serviceProvider, messageEnvelope, ct);
    }
}