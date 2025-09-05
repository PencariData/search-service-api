namespace SearchService.Shared.Models;

public class CachingOptions
{
    public int SuggestionCacheDurationMinutes { get; set; }
    public int ResultCacheDurationMinutes { get; set; }
}