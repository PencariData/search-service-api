namespace SearchService.Shared.Models;

public class ElasticConfiguration
{
    public string ElasticUrl { get; init; } = string.Empty;
    public string AccommodationIndex { get; init; } = string.Empty;
    public string DestinationIndex { get; init; } = string.Empty;
}