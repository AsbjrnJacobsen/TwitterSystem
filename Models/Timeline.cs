using System.Text.Json.Serialization;

namespace Models;

public class Timeline
{
    [JsonInclude] public List<Post>? Tweets { get; set; }
}