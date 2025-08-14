using System.Text.Json;
using SearchService.Domain.ValueObjects;

namespace SearchService.Infrastructure.Helpers.Parsing;

public static class CoordinateParser
{
    public static Coordinate Parse(JsonElement root)
    {
        if (!root.TryGetProperty("coordinate", out var coordElement))
        {
            return Coordinate.Create(0, 0);
        }

        // Handle different coordinate formats from Elasticsearch
        switch (coordElement.ValueKind)
        {
            // GeoPoint format: {"lat": 40.7128, "lon": -74.0060}
            case JsonValueKind.Object when coordElement.TryGetProperty("lat", out var latElement) && 
                                           coordElement.TryGetProperty("lon", out var lonElement):
                return Coordinate.Create(
                    latitude: latElement.GetDouble(),
                    longitude: lonElement.GetDouble()
                );
            case JsonValueKind.String:
            {
                // String format: "40.7128,-74.0060" or "lat,lon"
                var coordString = coordElement.GetString();
                if (!string.IsNullOrEmpty(coordString))
                {
                    var parts = coordString.Split(',');
                    if (parts.Length == 2 && 
                        double.TryParse(parts[0], out var lat) && 
                        double.TryParse(parts[1], out var lon))
                    {
                        return Coordinate.Create(latitude: lat, longitude: lon);
                    }
                }

                break;
            }
            case JsonValueKind.Array:
            {
                // Array format: [longitude, latitude] (GeoJSON format)
                var coords = coordElement.EnumerateArray().ToArray();
                if (coords.Length == 2)
                {
                    return Coordinate.Create(
                        latitude: coords[1].GetDouble(),  // Note: GeoJSON is [lon, lat]
                        longitude: coords[0].GetDouble()
                    );
                }

                break;
            }
        }
        
        // Fallback
        return Coordinate.Create(0, 0);
    }
}