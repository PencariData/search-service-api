namespace SearchService.Application.Dto;

public record SearchResultDto<T>(
    IReadOnlyList<T> Results,
    int Total);