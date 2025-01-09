using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BP.AWS.Messaging;

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
        _handlers.Add(new HandlerConfiguration(name, typeof(THandler), typeof(TMessage)));
        return this;
    }

    internal void RegisterHandlers(IServiceCollection services)
    {
        foreach (var handler in _handlers)
        {
            services.TryAddKeyedTransient(typeof(IHandler), handler.Name, handler.HandlerType);
        }
    }

    public string? GetQueueUrl<T>()
    {
        if (_queueMapping.TryGetValue(GetName<T>(), out var typeConfig))
        {
            var queueUrl = typeConfig.QueueUrl;
            if (!queueUrl.StartsWith("https"))
            {
                return $"https://sqs.{_region}.amazonaws.com/{_accountId}/{queueUrl}";
            }

            return queueUrl;
        }

        return null;
    }
}