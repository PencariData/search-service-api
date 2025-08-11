

using SearchService.Application.Interfaces;
using SearchService.Application.Interfaces.Repositories;
using SearchService.Domain.Entities;

namespace SearchService.Infrastructure.Repositories;

public class AccommodationRepository : IAccommodationRepository
{
    public Task<IEnumerable<AccommodationEntity>> GetByNameAsync(string name, int limit)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<AccommodationEntity>> GetByDestinationAsync(string destination, int limit)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<AccommodationEntity>> GetHotelSuggestionsAsync(string name, int limit)
    {
        throw new NotImplementedException();
    }
}