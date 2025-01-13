namespace BP.Messaging.AWS;

public class MessageBusOptions
{
    private readonly Dictionary<string, TypeConfiguration> _queueMapping = [];
    private string? _accountId;
    private string? _region;

    private record TypeConfiguration(string Name, string QueueUrl, Type Type);

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

    public MessageBusOptions MapTypeToQueue<T>(string queueUrl)
    {
        var messageTypeName = typeof(T).Name;
        if (string.IsNullOrWhiteSpace(messageTypeName)) throw new Exception();
        _queueMapping[messageTypeName] = new TypeConfiguration(messageTypeName, queueUrl, typeof(T));
        return this;
    }

    public string? GetQueueUrl(string eventType)
    {
        if (_queueMapping.TryGetValue(eventType, out var typeConfig))
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