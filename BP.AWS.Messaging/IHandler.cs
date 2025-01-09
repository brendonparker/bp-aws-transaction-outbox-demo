namespace BP.AWS.Messaging;

public interface IHandler<in T>
{
    Task<bool> HandleAsync(T message, CancellationToken cancellationToken);
}