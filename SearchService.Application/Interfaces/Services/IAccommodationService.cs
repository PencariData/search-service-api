using SearchService.Application.Dto;

namespace SearchService.Application.Interfaces.Services;

public interface IAccommodationService
{
    public Task<List<GetAccommodationResponse>> SearchAccommodationsAsync(GetAccommodationRequest request);
}