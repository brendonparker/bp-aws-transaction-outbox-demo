namespace BP.AWS.Messaging;

internal delegate Task<bool> HandlerWrapper(
    IServiceProvider serviceProvider,
    MessageEnvelope messageEnvelope,
    CancellationToken ct);