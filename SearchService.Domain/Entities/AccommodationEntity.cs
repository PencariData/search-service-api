using System.Text.Json.Serialization;
using SearchService.Domain.Enums;
using SearchService.Domain.ValueObjects;

namespace SearchService.Domain.Entities;

public class AccommodationEntity
{
    public Guid Id { get; set; }
    public string Name { get; private set; }
    public string DestinationName { get; private set; } // Jakarta
    public string FullDestination { get; private set; } // Jakarta, Indonesia
    public AccommodationType AccommodationType { get; private set; } // Hotel / Villa
    public Coordinate Coordinate { get; private set; }
    
    private AccommodationEntity(
        Guid id, 
        string name,
        string destinationName,
        string fullDestination,
        AccommodationType accommodationType,
        Coordinate coordinate)
    {
        Id = id;
        Name = name;
        DestinationName = destinationName;
        FullDestination = fullDestination;
        AccommodationType = accommodationType;
        Coordinate = coordinate;
    }
    
    public static AccommodationEntity Create(
        Guid id,
        string name, 
        string destinationName,
        string fullDestination,
        AccommodationType accommodationType,
        Coordinate coordinate)
    {
        if (string.IsNullOrEmpty(name))
            throw new  ArgumentNullException(nameof(name), "Accommodation name cannot be null or empty");
        
        if(string.IsNullOrEmpty(destinationName))
            throw new ArgumentNullException(nameof(destinationName), "Accommodation destinationName cannot be null or empty");
        
        if(string.IsNullOrEmpty(fullDestination))
            throw new ArgumentNullException(nameof(fullDestination), "Accommodation destination cannot be null or empty");
        
        return new AccommodationEntity(
            id, 
            name, 
            destinationName,
            fullDestination, 
            accommodationType,
            coordinate);
    }
}