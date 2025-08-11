using SearchService.Domain.Entities;

namespace SearchService.Application.Interfaces.Repositories;

public interface IAccommodationRepository
{
    public Task<IEnumerable<AccommodationEntity>> GetByNameAsync(string name, int limit);
    public Task<IEnumerable<AccommodationEntity>> GetByDestinationAsync(string destination, int limit);
    public Task<IEnumerable<AccommodationEntity>> GetHotelSuggestionsAsync(string name, int limit);
}