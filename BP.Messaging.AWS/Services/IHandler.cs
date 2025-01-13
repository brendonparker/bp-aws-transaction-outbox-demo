namespace BP.Messaging.AWS.Services;

public interface IHandler<in T>
{
    Task<bool> HandleAsync(T message, CancellationToken cancellationToken);
}