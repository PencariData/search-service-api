using SearchService.Application.Dto;
using SearchService.Domain.Entities;

namespace SearchService.Application.Interfaces.Repositories;

public interface IAccommodationRepository
{
    public Task<SearchResultDto<AccommodationEntity>> GetByFieldAsync(string field, string query, int page, int limit);
    public Task<SearchResultDto<AccommodationEntity>> GetByMultipleFieldAsync(List<string> fields, string query, int page, int limit);
    public Task<IEnumerable<string>> GetSuggestionsAsync(string name, int limit);
}