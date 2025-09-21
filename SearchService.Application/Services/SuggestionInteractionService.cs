using SearchService.Application.Dto;
using SearchService.Application.Interfaces.Services;
using SearchService.Domain.Events;

namespace SearchService.Application.Services;

public class SuggestionInteractionService(
    ILogQueueService<SearchEvent> logQueueService) 
    : ISuggestionInteractionService
{
    public Task RegisterClickAsync(PostSuggestionClickRequest request)
    {
        var evt = new SuggestionClicked(
            sessionId: request.SessionId,
            searchId: request.SearchId,
            itemIndex: request.ItemIndex
        );
        
        logQueueService.Enqueue(evt);
        
        return Task.CompletedTask;
    }
}