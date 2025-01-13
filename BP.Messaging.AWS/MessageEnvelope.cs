using System.Text.Json;

namespace BP.Messaging.AWS;

public class MessageEnvelope
{
    public static MessageEnvelope Create<T>(T content) =>
        new()
        {
            Type = typeof(T).Name,
            Payload = JsonSerializer.SerializeToElement(content)
        };

    public required string Type { get; set; }
    public required JsonElement Payload { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}