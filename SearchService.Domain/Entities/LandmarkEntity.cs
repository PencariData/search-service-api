using SearchService.Domain.Enums;
using SearchService.Domain.ValueObjects;

namespace SearchService.Domain.Entities;

public class LandmarkEntity
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public LandmarkType  LandmarkType { get; private set; }
    public Coordinate Coordinate { get; private set; }

    private LandmarkEntity(Guid id, string name, LandmarkType landmarkType, Coordinate coordinate)
    {
        Id = id;
        Name = name;
        LandmarkType = landmarkType;
        Coordinate = coordinate;
    }

    public static LandmarkEntity Create(string name, LandmarkType landmarkType, Coordinate coordinate)
    {
        if(string.IsNullOrEmpty(name))
            throw new ArgumentNullException(nameof(name), "Landmark name cannot be null or empty");

        return new LandmarkEntity(Guid.NewGuid(), name, landmarkType, coordinate);
    }
}