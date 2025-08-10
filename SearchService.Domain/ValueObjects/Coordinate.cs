namespace SearchService.Domain.ValueObjects;

public class Coordinate
{
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }

    private Coordinate(
        double latitude, 
        double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    public static Coordinate Create(double latitude, double longitude)
    {
        if (latitude < -90 || latitude > 90)
        {
            throw new ArgumentOutOfRangeException(nameof(latitude), "Latitude must be between -90 and 90.");
        }

        if (longitude < -180 || longitude > 180)
        {
            throw new ArgumentOutOfRangeException(nameof(longitude), "Longitude must be between -180 and 180.");
        }

        return new Coordinate(latitude, longitude);
    }
}