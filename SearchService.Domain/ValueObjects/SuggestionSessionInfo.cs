namespace SearchService.Domain.ValueObjects;

public record SuggestionSessionInfo(
    DateTime Timestamp,
    string Query,
    int AccommodationSuggestionCount,
    int DestinationSuggestionCount
);