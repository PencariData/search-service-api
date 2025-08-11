using SearchService.Application.Dtos;

namespace SearchService.Application.Interfaces.Services;

public interface IAccommodationService
{
    public Task<List<AccommodationDto>> SearchAccommodationAsync(GetAccommodationRequest request);
}