using SearchService.Application.Dto;

namespace SearchService.Application.Interfaces.Services;

public interface ISearchSuggestionService
{
    public Task<GetSuggestionResponse> GetSuggestionsAsync(GetSuggestionRequest request);
}