using BP.Messaging.AWS.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BP.Messaging.AWS.Implementations;

internal class HandlerInvoker(IServiceProvider serviceProvider) : IHandlerInvoker
{
    public async Task InvokeAsync(MessageEnvelope messageEnvelope, CancellationToken ct)
    {
        var handler = serviceProvider.GetRequiredKeyedService<HandlerWrapper>(messageEnvelope.Type);
        await handler(serviceProvider, messageEnvelope, ct);
    }
}