using SearchService.Application.Dto;

namespace SearchService.Application.Interfaces.Services;

public interface IAccommodationSearchService
{
    public Task<GetAccommodationResponse> SearchAccommodationsAsync(GetAccommodationRequest request);
}