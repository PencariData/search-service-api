namespace SearchService.Application.Dto;

public record SearchMetaDto(
    Guid SearchId,
    Guid SessionId,
    int Page,
    int ResulCount,
    int TotalResult
);