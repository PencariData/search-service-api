namespace SearchService.Domain.Entities;

public class ClickLogEntity
{
    public Guid Id { get; private set; }
    public Guid SearchLogId { get; private set; }
    public string DocumentId { get; private set; }
    public int Rank { get; private set; }
    public DateTime Timestamp { get; private set; }

    private ClickLogEntity() { } // EF
    
    private ClickLogEntity(
        Guid searchLogId, 
        string documentId, 
        int rank)
    {
        Id = Guid.NewGuid();
        SearchLogId = searchLogId;
        DocumentId = documentId;
        Rank = rank;
        Timestamp = DateTime.UtcNow;
    }

    public static ClickLogEntity Create(
        Guid searchLogId,
        string documentId,
        int rank)
    {
        if(searchLogId == Guid.Empty)
            throw new ArgumentNullException(nameof(searchLogId), "Search Log Id cannot be empty.");
        
        if(documentId == null)
            throw new ArgumentNullException(nameof(documentId), "Document Id cannot be empty.");
        
        return new ClickLogEntity(
            searchLogId,
            documentId,
            rank);
    }
}