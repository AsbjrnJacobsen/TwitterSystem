using System.Text.Json.Serialization;

namespace Models;

public class Timeline
{
    [JsonInclude]
    [JsonPropertyName("tweets")]
    public List<Post>? Tweets { get; set; }
}