
using System.Text.Json.Serialization;

namespace BP.TransactionalOutboxDemo.Infrastructure;

public class DatabaseConfig
{
    [JsonPropertyName("username")]
    public string? Username { get; set; }
    [JsonPropertyName("password")]
    public string? Password { get; set; }
    [JsonPropertyName("dbname")]
    public string? DbName { get; set; }
    [JsonPropertyName("host")]
    public string? Host { get; set; }
}