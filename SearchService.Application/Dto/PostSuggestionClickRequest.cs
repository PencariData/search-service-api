namespace SearchService.Application.Dto;

public record PostSuggestionClickRequest
(
    Guid SessionId,
    Guid SearchId,
    int ItemIndex
);