using SearchService.Application.Dto;

namespace SearchService.Application.Interfaces.Services;

public interface ISuggestionInteractionService
{
    public Task RegisterClickAsync(PostSuggestionClickRequest request);
}