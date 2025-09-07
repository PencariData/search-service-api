using SearchService.Domain.Enums;

namespace SearchService.Domain.Entities;

public class SearchLogEntity
{
    public Guid Id { get; private set; }
    public Guid SearchId { get; private set; }
    public DateTime Timestamp { get; private set; }
    public string SearchQuery { get; private set; } = null!;
    public string SearchType { get; private set; }
    public int Page { get; private set; }
    public int ResultCount { get; private set; }
    public int TotalResultCount { get; private set; }
    public bool IsFromCache { get; private set; }
    public long ElapsedMs { get; private set; }
    
    // Optional relevance signals
    public Guid? ClickedResultId { get; private set; }
    public int? ClickRank { get; private set; }
    public long? DwellTimeMs { get; private set; }
    
    // Validation filter
    public LogValidity Validity { get; private set; }
    public string? InvalidReason { get; private set; }
    
    private SearchLogEntity() { } // EF
    
    private SearchLogEntity( 
        Guid searchId,
        DateTime timestamp,
        string searchQuery,
        string searchType,
        int page,
        int resultCount,
        int totalResultCount,
        bool isFromCache,
        long elapsedMs,
        Guid? clickedResultId,
        int? clickRank,
        long? dwellTimeMs,
        LogValidity validity,
        string? invalidReason
    )
    {
        Id = Guid.NewGuid();
        SearchId = searchId;
        Timestamp = timestamp;
        SearchQuery = searchQuery;
        SearchType = searchType;
        Page = page;
        ResultCount = resultCount;
        TotalResultCount = totalResultCount;
        IsFromCache = isFromCache;
        ElapsedMs = elapsedMs;
        ClickedResultId = clickedResultId;
        ClickRank = clickRank;
        DwellTimeMs = dwellTimeMs;
        Validity = validity;
        InvalidReason = invalidReason;
    }

    public static SearchLogEntity Create(
        Guid searchId,
        DateTime timestamp,
        string searchQuery,
        string searchType,
        int page,
        int resultCount,
        int totalResultCount,
        bool isFromCache,
        long elapsedMs,
        Guid? clickedResultId,
        int? clickRank,
        long? dwellTimeMs,
        LogValidity validity,
        string? invalidReason)
    {
        if (searchId == Guid.Empty)
            throw new ArgumentNullException(nameof(searchId), "SearchId cannot be empty");
        
        if (timestamp == DateTime.MinValue)
            throw new ArgumentOutOfRangeException(nameof(timestamp), "Timestamp cannot be empty");
        
        if (string.IsNullOrWhiteSpace(searchQuery))
            throw new ArgumentNullException(nameof(searchQuery), "QueryText cannot be empty");
        
        if(string.IsNullOrWhiteSpace(searchType))
            throw new ArgumentNullException(nameof(searchType), "SearchType cannot be empty");
        
        if(page < 0)
            throw new ArgumentOutOfRangeException(nameof(page), "Page cannot be negative");
        
        if(resultCount < 0)
            throw new ArgumentOutOfRangeException(nameof(resultCount), "ResultCount cannot be negative");
        
        if(totalResultCount < 0)
            throw new  ArgumentOutOfRangeException(nameof(totalResultCount), "TotalResultCount cannot be negative");
        
        if(elapsedMs < 0)
            throw new ArgumentOutOfRangeException(nameof(elapsedMs), "ElapsedMs cannot be negative");
        
        return new SearchLogEntity(
            searchId, 
            timestamp, 
            searchQuery, 
            searchType, 
            page,
            resultCount, 
            totalResultCount,
            isFromCache, 
            elapsedMs, 
            clickedResultId ?? Guid.Empty, 
            clickRank ?? -1,
            dwellTimeMs ?? -1,
            validity, 
            invalidReason);
    }

    public void UpdateSearchId(Guid searchId)
    {
        if  (searchId == Guid.Empty)
            throw new ArgumentNullException(nameof(searchId), "SearchId cannot be empty");
        
        SearchId = searchId;
    }
}