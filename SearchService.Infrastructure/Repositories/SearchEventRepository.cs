using System.Text.Json;
using SearchService.Application.Interfaces.Repositories;
using SearchService.Domain.Events;
using SearchService.Infrastructure.Entities;
using SearchService.Infrastructure.Persistence;

namespace SearchService.Infrastructure.Repositories;

public class SearchEventRepository(AppDbContext db) : ISearchEventRepository
{
    public async Task AddAsync(SearchEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var entity = new SearchEventEntity
        {
            EventId = domainEvent.EventId,
            SessionId = domainEvent.SessionId,
            SearchId = (domainEvent is SearchScopedEvent scoped ? scoped.SearchId : null),
            EventType = domainEvent.EventType,
            Payload = JsonSerializer.Serialize(domainEvent.ToPayload()),
            OccurredAt = domainEvent.OccurredAt
        };

        db.SearchEvents.Add(entity);
        await db.SaveChangesAsync(cancellationToken);
    }
}
