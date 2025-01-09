using System.Text.Json;

namespace BP.AWS.Messaging;

public class MessageEnvelope
{
    public static string CreateJson<T>(T content) =>
        JsonSerializer.Serialize(Create(content));

    public static MessageEnvelope Create<T>(T content) =>
        new()
        {
            Type = content!.GetType().AssemblyQualifiedName!,
            Payload = JsonSerializer.SerializeToElement(content)
        };

    public required string Type { get; set; }
    public required JsonElement Payload { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}