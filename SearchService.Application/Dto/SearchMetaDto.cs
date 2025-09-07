namespace SearchService.Application.Dto;

public record SearchMetaDto(
    Guid SearchId,
    int Page,
    int ResulCount,
    int TotalResult
);