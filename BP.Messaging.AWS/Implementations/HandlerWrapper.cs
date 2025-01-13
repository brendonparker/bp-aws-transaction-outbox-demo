namespace BP.Messaging.AWS.Implementations;

internal delegate Task<bool> HandlerWrapper(
    IServiceProvider serviceProvider,
    MessageEnvelope messageEnvelope,
    CancellationToken ct);