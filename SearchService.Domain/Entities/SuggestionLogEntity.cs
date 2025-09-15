using SearchService.Domain.ValueObjects;

namespace SearchService.Domain.Entities;

public class SuggestionLogEntity
{
    public Guid SuggestionId { get; private set; }
    public Guid SearchId { get; private set; }
    public SuggestionSessionInfo Session { get; private set; }
    public SuggestionPerformanceInfo Performance { get; private set; }
    public SuggestionInteractionInfo? Interaction { get; private set; }

    private SuggestionLogEntity() { } // EF

    private SuggestionLogEntity(
        Guid suggestionId,
        Guid searchId,
        SuggestionSessionInfo session,
        SuggestionPerformanceInfo performance,
        SuggestionInteractionInfo? interaction = null)
    {
        SuggestionId = suggestionId == Guid.Empty 
            ? throw new ArgumentNullException(nameof(suggestionId)) 
            : suggestionId;

        SearchId = searchId == Guid.Empty 
            ? throw new ArgumentNullException(nameof(searchId)) 
            : searchId;

        Session = session ?? throw new ArgumentNullException(nameof(session));
        Performance = performance ?? throw new ArgumentNullException(nameof(performance));
        Interaction = interaction;
    }

    public static SuggestionLogEntity Create(
        Guid suggestionId,
        Guid searchId,
        SuggestionSessionInfo session,
        SuggestionPerformanceInfo performance,
        SuggestionInteractionInfo? interaction = null)
        => new(suggestionId, searchId, session, performance, interaction);

    public void AddInteraction(SuggestionInteractionInfo interaction)
    {
        Interaction = interaction;
    }
}
