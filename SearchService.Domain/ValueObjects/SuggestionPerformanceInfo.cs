namespace SearchService.Domain.ValueObjects;

public record SuggestionPerformanceInfo(
    bool IsFromCache,
    long ElapsedMs
);