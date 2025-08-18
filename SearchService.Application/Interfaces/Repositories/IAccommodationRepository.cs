using SearchService.Domain.Entities;

namespace SearchService.Application.Interfaces.Repositories;

public interface IAccommodationRepository
{
    public Task<IEnumerable<AccommodationEntity>> GetByFieldAsync(string field, string query, int limit);
    public Task<IEnumerable<AccommodationEntity>> GetByMultipleFieldAsync(List<string> fields, string query, int limit);
    public Task<IEnumerable<string>> GetSuggestionsAsync(string name, int limit);
}