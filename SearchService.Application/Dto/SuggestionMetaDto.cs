namespace SearchService.Application.Dto;

public record SuggestionMetaDto
(
    Guid SearchId,
    int accommodationSuggestionCount,
    int destinationSuggestionCount
);