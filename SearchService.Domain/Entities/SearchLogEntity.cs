namespace SearchService.Domain.Entities;

public class SearchLogEntity
{
    public Guid LogId { get; private set; }        // Primary key
    public Guid SessionId { get; private set; }    // Groups by session
    public Guid SearchId { get; private set; }     // Identifies one logical search
    public DateTime Timestamp { get; private set; }
    public string Query { get; private set; }
    public int Page { get; private set; }
    public int ResultCount { get; private set; }
    public long ElapsedMs { get; private set; }
    public string? UserAgent { get; private set; }
    public string? IpAddress { get; private set; }
    public string? Referer { get; private set; }

    private SearchLogEntity() { } // EF

    private SearchLogEntity(Guid sessionId, Guid searchId, DateTime timestamp,
        string query, int page, int resultCount, long elapsedMs,
        string? userAgent, string? ipAddress, string? referer)
    {
        if (string.IsNullOrWhiteSpace(query))
            throw new ArgumentException("Query cannot be empty", nameof(query));

        LogId = Guid.NewGuid();
        SessionId = sessionId;
        SearchId = searchId;
        Timestamp = timestamp;
        Query = query.Trim();
        Page = page >= 1 ? page : 1;
        ResultCount = resultCount;
        ElapsedMs = elapsedMs;
        UserAgent = userAgent;
        IpAddress = ipAddress;
        Referer = referer;
    }

    public static SearchLogEntity Create(
        Guid sessionId,
        Guid searchId,
        string query,
        int page,
        int resultCount,
        long elapsedMs,
        string? userAgent,
        string? ipAddress,
        string? referer) =>
        new(sessionId, searchId, DateTime.UtcNow, query, page, resultCount, elapsedMs, userAgent, ipAddress, referer);
}