using System.Text.Json.Serialization;
using SearchService.Domain.Enums;
using SearchService.Domain.ValueObjects;

namespace SearchService.Domain.Entities;

public class AccommodationEntity
{
    public Guid Id { get; set; }
    public string Name { get; private set; }
    public string Country { get; private set; } // Indonesia
    public string AdministrationLevel1 { get; private set; } // Bali
    public string AdministrationLevel2 { get; private set; } // Gianyar
    public string FullDestination { get; private set; } // Gianyar, Bali, Indonesia
    public AccommodationType AccommodationType { get; private set; } // Hotel / Villa
    public Coordinate Coordinate { get; private set; }
    
    private AccommodationEntity(
        Guid id, 
        string name,
        string country,
        string administrationLevel1,
        string administrationLevel2,
        string fullDestination,
        AccommodationType accommodationType,
        Coordinate coordinate)
    {
        Id = id;
        Name = name;
        Country = country;
        AdministrationLevel1 = administrationLevel1;
        AdministrationLevel2 = administrationLevel2;
        FullDestination = fullDestination;
        AccommodationType = accommodationType;
        Coordinate = coordinate;
    }
    
    public static AccommodationEntity Create(
        Guid id,
        string name, 
        string country, 
        string administrationLevel1, 
        string administrationLevel2, 
        string fullDestination,
        AccommodationType accommodationType,
        Coordinate coordinate)
    {
        if (string.IsNullOrEmpty(name))
            throw new  ArgumentNullException(nameof(name), "Accommodation name cannot be null or empty");
        
        if(string.IsNullOrEmpty(country))
            throw new ArgumentNullException(nameof(country), "Accommodation country cannot be null or empty");
        
        if(string.IsNullOrEmpty(fullDestination))
            throw new ArgumentNullException(nameof(fullDestination), "Accommodation destination cannot be null or empty");
        
        return new AccommodationEntity(
            id, 
            name, 
            country, 
            administrationLevel1, 
            administrationLevel2, 
            fullDestination, 
            accommodationType,
            coordinate);
    }
}