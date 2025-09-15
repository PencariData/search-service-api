namespace SearchService.Domain.ValueObjects;

public record SearchPerformanceInfo(
    bool IsFromCache,
    long ElapsedMs
);