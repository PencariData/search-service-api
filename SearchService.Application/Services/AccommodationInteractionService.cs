using SearchService.Application.Dto;
using SearchService.Application.Interfaces.Services;
using SearchService.Domain.Events;

namespace SearchService.Application.Services;

public class AccommodationInteractionService(
    ILogQueueService<SearchEvent> logQueueService)
    : IAccommodationInteractionService
{
    public Task RegisterClickAsync(PostAccommodationClickRequest request)
    {
        var evt = new ResultClicked(
            sessionId: request.SessionId,
            searchId: request.SearchId,
            itemIndex: request.ItemIndex
        );

        logQueueService.Enqueue(evt);

        return Task.CompletedTask; // fire & forget
    }
}

