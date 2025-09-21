namespace SearchService.Domain.Events;

#region Abstraction

public abstract class SearchEvent(Guid sessionId, Guid? searchId = null)
{
    public Guid EventId { get; } = Guid.NewGuid();
    public Guid SessionId { get; } = sessionId;
    public Guid? SearchId { get; } = searchId;
    public DateTime OccurredAt { get; } = DateTime.UtcNow;

    public abstract string EventType { get; }

    public virtual object ToPayload()
    {
        var props = GetType()
            .GetProperties()
            .Where(p => p.DeclaringType == GetType() && p.Name != nameof(EventType));

        return props.ToDictionary(p => p.Name, p => p.GetValue(this));
    }
}

#endregion

#region Implementation
public sealed class SearchPerformed(
    Guid sessionId,
    Guid searchId,
    string query,
    int page,
    int resultCount,
    long elapsedMs)
    : SearchEvent(sessionId, searchId)
{
    public string Query { get; } = query;
    public int Page { get; } = page;
    public int ResultCount { get; } = resultCount;
    public long ElapsedMs { get; } = elapsedMs;

    public override string EventType => nameof(SearchPerformed);
}

public sealed class ResultClicked : SearchEvent
{
    public int ItemIndex { get; }

    public ResultClicked(Guid sessionId, Guid searchId, int itemIndex)
        : base(sessionId, searchId)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(itemIndex);
        ItemIndex = itemIndex;
    }

    public override string EventType => nameof(ResultClicked);
}

public sealed class SuggestionsShown(Guid sessionId, Guid searchId, IEnumerable<string> suggestions)
    : SearchEvent(sessionId, searchId)
{
    public IReadOnlyList<string> Suggestions { get; } = suggestions.ToList();

    public override string EventType => nameof(SuggestionsShown);
}

public sealed class SuggestionClicked : SearchEvent
{
    public int ItemIndex { get; }

    public SuggestionClicked(Guid sessionId, Guid searchId, int itemIndex)
        : base(sessionId, searchId)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(itemIndex);
        ItemIndex = itemIndex;
    }

    public override string EventType => nameof(SuggestionClicked);
}


#endregion