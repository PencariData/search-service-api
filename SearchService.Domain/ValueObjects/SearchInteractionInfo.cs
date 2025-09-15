namespace SearchService.Domain.ValueObjects;

public record SearchInteractionInfo(
    Guid ClickedResultId,
    int ClickRank,
    long DwellTimeMs
);