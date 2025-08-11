using SearchService.Domain.ValueObjects;

namespace SearchService.Domain.Entities;

public class DestinationEntity
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Country { get; private set; }
    public string AdministrationLevel1 { get; private set; }
    public string AdministrationLevel2 { get; private set; }
    public Coordinate CenterCoordinate { get; private set; }
    public int? AccommodationCount { get; private set; }

    private DestinationEntity(
        Guid id, 
        string name,
        string country,
        string administrationLevel1,
        string administrationLevel2,
        Coordinate centerCoordinate,
        int? accommodationCount)
    {
        Id = id;
        Name = name;
        Country = country;
        AdministrationLevel1 = administrationLevel1;
        AdministrationLevel2 = administrationLevel2;
        CenterCoordinate = centerCoordinate;
        AccommodationCount = accommodationCount;
    }

    public static DestinationEntity Create(
        string name, 
        string country,
        string administrationLevel1,
        string administrationLevel2,
        Coordinate centerCoordinate,
        int? accommodationCount)
    {
        if(string.IsNullOrEmpty(name))
            throw new ArgumentNullException(nameof(name), "Region name cannot be null or empty.");
        
        if(string.IsNullOrWhiteSpace(country))
            throw new ArgumentNullException(nameof(country), "Region country cannot be null or empty.");
        
        return new DestinationEntity(
            Guid.NewGuid(),name, 
            country, 
            administrationLevel1, 
            administrationLevel2, 
            centerCoordinate, 
            accommodationCount);
    }
}