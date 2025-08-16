using SearchService.Domain.ValueObjects;

namespace SearchService.Domain.Entities;

public class DestinationEntity
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } // Gianyar
    public string FullName { get; private set; } // Gianyar, Bali, Indonesia
    public Coordinate CenterCoordinate { get; private set; }
    public int? AccommodationCount { get; private set; }

    private DestinationEntity(
        Guid id, 
        string name,
        string fullName,
        Coordinate centerCoordinate,
        int? accommodationCount)
    {
        Id = id;
        Name = name;
        FullName = fullName;
        CenterCoordinate = centerCoordinate;
        AccommodationCount = accommodationCount;
    }

    public static DestinationEntity Create(
        string name,
        string fullName,
        Coordinate centerCoordinate,
        int? accommodationCount)
    {
        if(string.IsNullOrEmpty(name))
            throw new ArgumentNullException(nameof(name), "Destination name cannot be null or empty.");
        
        if(string.IsNullOrEmpty(fullName))
            throw new ArgumentNullException(nameof(fullName), "Destination full name cannot be null or empty.");
        
        return new DestinationEntity(
            Guid.NewGuid(),
            name,
            fullName,
            centerCoordinate, 
            accommodationCount);
    }
}