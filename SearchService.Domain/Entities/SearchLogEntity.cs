using SearchService.Domain.Enums;
using SearchService.Domain.ValueObjects;

namespace SearchService.Domain.Entities;

public class SearchLogEntity
{
    public Guid SearchId { get; private set; }
    public SearchSessionInfo Session { get; private set; }
    public SearchPerformanceInfo Performance { get; private set; }
    public SearchInteractionInfo? Interaction { get; private set; }

    private SearchLogEntity() { } // EF

    private SearchLogEntity(
        Guid searchId,
        SearchSessionInfo session,
        SearchPerformanceInfo performance,
        SearchInteractionInfo? interaction = null)
    {
        SearchId = searchId == Guid.Empty 
            ? throw new ArgumentNullException(nameof(searchId)) 
            : searchId;

        Session = session ?? throw new ArgumentNullException(nameof(session));
        Performance = performance ?? throw new ArgumentNullException(nameof(performance));
        Interaction = interaction;
    }

    public static SearchLogEntity Create(
        Guid searchId,
        SearchSessionInfo session,
        SearchPerformanceInfo performance,
        SearchInteractionInfo? interaction = null)
        => new(searchId, session, performance, interaction);

    public void AddInteraction(SearchInteractionInfo interaction)
    {
        Interaction = interaction;
    }
}