using SearchService.Domain.ValueObjects;

namespace SearchService.Domain.Entities;

public class AccommodationEntity
{
    public Guid Id { get; set; }
    public string Name { get; private set; }
    public string Address { get; private set; }
    public Coordinate Coordinate { get; private set; }
    
    private AccommodationEntity(
        Guid id, 
        string name, 
        string address, 
        Coordinate coordinate)
    {
        Id = id;
        Name = name;
        Address = address;
        Coordinate = coordinate;
    }
    
    public static AccommodationEntity Create(string name, string address, Coordinate coordinate)
    {
        if (string.IsNullOrEmpty(name))
            throw new  ArgumentNullException(nameof(name), "Accommodation name cannot be null or empty");
        
        if(string.IsNullOrEmpty(address))
            throw new ArgumentNullException(nameof(address), "Accommodation address cannot be null or empty");

        return new AccommodationEntity(Guid.NewGuid(), name, address, coordinate);
    }
}