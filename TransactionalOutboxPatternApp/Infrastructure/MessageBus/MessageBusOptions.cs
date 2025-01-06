namespace TransactionalOutboxPatternApp.Infrastructure.MessageBus;

public class MessageBusOptions
{
    private readonly Dictionary<string, string> _queueMapping = [];
    private string? _accountId;
    private string? _region;

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
        _queueMapping[messageTypeName] = queueUrl;
        return this;
    }

    public string? GetQueueUrl<T>()
    {
        if (_queueMapping.TryGetValue(GetName<T>(), out var queueUrl))
        {
            if (!queueUrl.StartsWith("https"))
            {
                queueUrl = $"https://sqs.${_region}.amazonaws.com/${_accountId}/{queueUrl}";
            }

            return queueUrl;
        }

        return null;
    }
}