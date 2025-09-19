namespace SearchService.Domain.Entities;

public class SuggestionLogEntity
{
    public Guid LogId { get; private set; }             // Primary Key
    public Guid SessionId { get; private set; }         // Groups by session
    public string Query { get; private set; }
    public int AccommodationSuggestionCount { get; private set; }
    public int DestinationSuggestionCount { get; private set; }
    public long ElapsedMs { get; private set; }
    public DateTime Timestamp { get; private set; }

    private SuggestionLogEntity() { } // EF

    private SuggestionLogEntity(Guid sessionId, string query, int accommodationSuggestionCount, int destinationSuggestionCount, long elapsedMs)
    {
        LogId = Guid.NewGuid();
        SessionId = sessionId;
        Query = query ?? throw new ArgumentNullException(nameof(query));
        AccommodationSuggestionCount = accommodationSuggestionCount;
        DestinationSuggestionCount = destinationSuggestionCount;
        ElapsedMs = elapsedMs;
        Timestamp = DateTime.UtcNow;
    }

    public static SuggestionLogEntity Create(Guid sessionId, string query, int accommodationSuggestionCount, int destinationSuggestionCount, long elapsedMs)
        => new(sessionId, query, accommodationSuggestionCount, destinationSuggestionCount, elapsedMs);
}