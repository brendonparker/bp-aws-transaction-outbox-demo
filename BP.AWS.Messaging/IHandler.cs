using System.Text.Json;

namespace BP.AWS.Messaging;

public interface IHandler
{
    Task<bool> HandleAsync(JsonElement message, CancellationToken cancellationToken);
}

public interface IHandler<in T> : IHandler
{
    Task<bool> HandleAsync(T message, CancellationToken cancellationToken);
}