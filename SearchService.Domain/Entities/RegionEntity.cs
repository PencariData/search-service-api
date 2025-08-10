namespace SearchService.Domain.Entities;

public class RegionEntity
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public int? AccommodationCount { get; private set; }

    private RegionEntity(Guid id, string name, int? accommodationCount)
    {
        Id = id;
        Name = name;
        AccommodationCount = accommodationCount;
    }

    public static RegionEntity Create(string name, int? accommodationCount)
    {
        if(string.IsNullOrEmpty(name))
            throw new ArgumentNullException(nameof(name), "Region name cannot be null or empty.");
        
        return new RegionEntity(Guid.NewGuid(), name,  accommodationCount);
    }
}