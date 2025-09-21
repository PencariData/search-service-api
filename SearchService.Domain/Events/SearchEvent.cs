namespace SearchService.Domain.Events;

public abstract class SearchEvent
{
    public Guid EventId { get; private set; } = Guid.NewGuid();
    public Guid SessionId { get; private set; }
    public DateTime OccurredAt { get; private set; } = DateTime.UtcNow;

    protected SearchEvent(Guid sessionId)
    {
        SessionId = sessionId;
    }

    public abstract string EventType { get; }
    public abstract object ToPayload();
}

public abstract class SearchScopedEvent : SearchEvent
{
    public Guid SearchId { get; private set; }

    protected SearchScopedEvent(Guid sessionId, Guid searchId)
        : base(sessionId)
    {
        SearchId = searchId;
    }
}

public sealed class SearchPerformed : SearchScopedEvent
{
    public string Query { get; }
    public int Page { get; }
    public int ResultCount { get; }
    public long ElapsedMs { get; }

    public SearchPerformed(Guid sessionId, Guid searchId, string query, int page, int resultCount, long elapsedMs)
        : base(sessionId, searchId)
    {
        Query = query;
        Page = page;
        ResultCount = resultCount;
        ElapsedMs = elapsedMs;
    }

    public override string EventType => nameof(SearchPerformed);

    public override object ToPayload() => new { Query, Page, ResultCount, ElapsedMs };
}

public sealed class ResultClicked : SearchScopedEvent
{
    public int ItemIndex { get; }

    public ResultClicked(Guid sessionId, Guid searchId, int itemIndex)
        : base(sessionId, searchId)
    {
        if (itemIndex < 0) throw new ArgumentOutOfRangeException(nameof(itemIndex));
        ItemIndex = itemIndex;
    }

    public override string EventType => nameof(ResultClicked);

    public override object ToPayload() => new { ItemIndex };
}

public sealed class SuggestionsShown : SearchEvent
{
    public IReadOnlyList<string> Suggestions { get; }

    public SuggestionsShown(Guid sessionId, IEnumerable<string> suggestions)
        : base(sessionId)
    {
        Suggestions = suggestions.ToList();
    }

    public override string EventType => nameof(SuggestionsShown);

    public override object ToPayload() => new { Suggestions };
}
