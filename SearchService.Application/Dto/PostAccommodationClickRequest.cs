namespace SearchService.Application.Dto;

public record PostAccommodationClickRequest(
    Guid SessionId,
    Guid SearchId,
    int ItemIndex
);