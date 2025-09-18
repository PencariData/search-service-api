using SearchService.Application.Dto;

namespace SearchService.Application.Interfaces.Services;

public interface IAccommodationService
{
    public Task<GetAccommodationResponse> SearchAccommodationsAsync(GetAccommodationRequest request, RequestInfoDto? requestInfo);
}