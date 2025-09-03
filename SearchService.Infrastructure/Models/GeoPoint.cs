using System.Text.Json.Serialization;

namespace SearchService.Infrastructure.Models;

public class GeoPoint
{
    [JsonPropertyName("lat")]
    public double Lat { get; set; }

    [JsonPropertyName("lon")]
    public double Lon { get; set; }
}