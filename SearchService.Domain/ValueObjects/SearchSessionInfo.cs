namespace SearchService.Domain.ValueObjects;

public record SearchSessionInfo(
    DateTime Timestamp,
    string Query,
    string Type,
    int Page,
    int ResultCount,
    int TotalResultCount
);