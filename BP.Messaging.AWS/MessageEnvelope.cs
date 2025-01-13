using System.Text.Json;

namespace BP.Messaging.AWS;

public class MessageEnvelope
{
    public const string DefaultMessageGroupId = "default";

    public static MessageEnvelope Create<T>(T content, string? messageGroupId = null) =>
        new()
        {
            Type = typeof(T).Name,
            Payload = JsonSerializer.SerializeToElement(content),
            MessageGroupId = messageGroupId ?? DefaultMessageGroupId
        };

    public string MessageGroupId { get; set; } = DefaultMessageGroupId;
    public required string Type { get; set; }
    public required JsonElement Payload { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}