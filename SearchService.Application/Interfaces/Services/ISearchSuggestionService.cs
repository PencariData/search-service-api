using SearchService.Application.Dto;

namespace SearchService.Application.Interfaces.Services;

public interface ISearchSuggestionService
{
    public Task<List<string>> SearchAccommodationSuggestionsAsync(GetAccommodationSuggestionRequest request);
}