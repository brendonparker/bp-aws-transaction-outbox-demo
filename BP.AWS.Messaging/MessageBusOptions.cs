using System.Text.Json;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TransactionalOutboxPatternApp.Domain;

namespace TransactionalOutboxPatternApp.Infrastructure.MessageBus;

public class MessageBusOptions
{
    private readonly Dictionary<string, TypeConfiguration> _queueMapping = [];
    private readonly HashSet<HandlerConfiguration> _handlers = [];
    private string? _accountId;
    private string? _region;

    private record TypeConfiguration(string Name, string QueueUrl, Type Type);

    private record HandlerConfiguration(string Name, Type HandlerType, Type EventType);

    public MessageBusOptions SetDefaultAccountId(string accountId)
    {
        _accountId = accountId;
        return this;
    }

    public MessageBusOptions SetDefaultRegion(string region)
    {
        _region = region;
        return this;
    }

    private string GetName<T>() =>
        typeof(T).AssemblyQualifiedName ?? string.Empty;

    public MessageBusOptions MapTypeToQueue<T>(string queueUrl)
    {
        var messageTypeName = GetName<T>();
        if (string.IsNullOrWhiteSpace(messageTypeName)) throw new Exception();
        _queueMapping[messageTypeName] = new TypeConfiguration(messageTypeName, queueUrl, typeof(T));
        return this;
    }

    public MessageBusOptions UseHandler<THandler, TMessage>() where THandler : IHandler<TMessage>
    {
        var name = GetName<TMessage>();
        _handlers.Add(new HandlerConfiguration(name, typeof(HandlerConfiguration), typeof(TMessage)));
        return this;
    }

    public void RegisterHandlers(IServiceCollection services)
    {
        foreach (var handler in _handlers)
        {
            services.TryAddKeyedTransient(typeof(IHandler<>), handler.Name, handler.HandlerType);
        }
    }

    public object Deserialize(MessageEnvelope envelope)
    {
        if (!_queueMapping.TryGetValue(envelope.Type, out var queueMapping))
        {
            throw new Exception($"No type mapped for: {envelope.Type}");
        }

        var result = JsonSerializer.Deserialize(envelope.Payload, queueMapping.Type);
        return result ?? throw new Exception($"Failed to deserialize type: {queueMapping.Type} {envelope.Payload}");
    }

    public string? GetQueueUrl<T>()
    {
        if (_queueMapping.TryGetValue(GetName<T>(), out var typeConfig))
        {
            var queueUrl = typeConfig.QueueUrl;
            if (!queueUrl.StartsWith("https"))
            {
                return $"https://sqs.{_region}.amazonaws.com/{_accountId}/{typeConfig}";
            }

            return queueUrl;
        }

        return null;
    }
}

public interface IMessageDispatcher
{
    Task DispatchAsync(object message);
}

public class MessageDispatcher(IServiceProvider serviceProvider) : IMessageDispatcher
{
    public async Task DispatchAsync(object message)
    {
        var key = message.GetType().AssemblyQualifiedName ?? throw new Exception();
        var handler = serviceProvider.GetRequiredKeyedService(typeof(IHandler<>), key);
        var method = typeof(IHandler<>).GetMethod(nameof(IHandler<object>.HandleAsync));
        var result = method!.Invoke(handler, [message, CancellationToken.None]) as Task<bool>;
        await result!;
    }
}