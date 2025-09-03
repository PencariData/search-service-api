using System.Text.Json.Serialization;
using Elastic.Clients.Elasticsearch.Mapping;

namespace SearchService.Infrastructure.Models;

public class Destination
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("name_suggest")]
    public NameSuggest NameSuggest { get; set; } = new();

    [JsonPropertyName("fullName")]
    public string FullName { get; set; } = string.Empty;

    [JsonPropertyName("centerCoordinate")]
    public GeoPoint CenterCoordinate { get; set; } = new();

    [JsonPropertyName("accommodationCount")]
    public int AccommodationCount { get; set; }
}