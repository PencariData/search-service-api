using System.Text.Json.Serialization;

namespace SearchService.Infrastructure.Models;

public class NameSuggest
{
    [JsonPropertyName("input")]
    public string[] Input { get; set; } = Array.Empty<string>();
}