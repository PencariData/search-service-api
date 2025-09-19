namespace SearchService.Application.Dto;

public record SuggestionMetaDto
(
    Guid SessionId,
    int accommodationSuggestionCount,
    int destinationSuggestionCount
);