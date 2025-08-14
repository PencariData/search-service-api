using System.Text.Json;
using SearchService.Domain.Entities;
using SearchService.Domain.Enums;
using SearchService.Infrastructure.Helpers.Parsing;

namespace SearchService.Infrastructure.Helpers.Mapping;

public static class JsonToAccommodationMapper
{
    public static AccommodationEntity Map(JsonDocument document)
    {
        var root = document.RootElement;
        
        // Extract basic properties
        var id = root.GetProperty("id").GetGuid();
        var name = root.GetProperty("name").GetString() ?? "";
        var country = root.GetProperty("country").GetString() ?? "";
        var adminLevel1 = root.GetProperty("administrationLevel1").GetString() ?? "";
        var adminLevel2 = root.GetProperty("administrationLevel2").GetString() ?? "";
        var fullDestination = root.GetProperty("fullDestination").GetString() ?? "";
        var accommodationType = root.GetProperty("accommodationType").GetString() ?? "";
        
        // Handle coordinate parsing
        var coordinate = CoordinateParser.Parse(root);
        
        // Handle enum parsing
        Enum.TryParse(accommodationType, out AccommodationType accommodationTypeEnum);
        
        return AccommodationEntity.Create(
            id: id,
            name: name,
            country: country,
            administrationLevel1: adminLevel1,
            administrationLevel2: adminLevel2,
            fullDestination: fullDestination,
            accommodationType: accommodationTypeEnum,
            coordinate: coordinate
        );
    }
}