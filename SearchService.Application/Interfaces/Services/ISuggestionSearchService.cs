using SearchService.Application.Dto;

namespace SearchService.Application.Interfaces.Services;

public interface ISuggestionSearchService
{
    public Task<GetSuggestionResponse> GetSuggestionsAsync(GetSuggestionRequest request);
}