namespace SearchService.Domain.ValueObjects;

public record SuggestionInteractionInfo(
    Guid ClickedResultId,
    int ClickRank
);