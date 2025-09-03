using System.Text.Json.Serialization;
namespace SearchService.Infrastructure.Models;

/// <summary>
/// This is accommodation document model
/// This model used for mapping data to elastic search index
/// </summary>
public class Accommodation
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("name_suggest")]
    public NameSuggest NameSuggest { get; set; } = new();

    [JsonPropertyName("destinationId")]
    public string DestinationId { get; set; } = string.Empty;

    [JsonPropertyName("destinationName")]
    public string DestinationName { get; set; } = string.Empty;

    [JsonPropertyName("fullDestination")]
    public string FullDestination { get; set; } = string.Empty;

    [JsonPropertyName("accommodationType")]
    public string AccommodationType { get; set; } = string.Empty;

    [JsonPropertyName("coordinate")]
    public GeoPoint Coordinate { get; set; } = new();
}