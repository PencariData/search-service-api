namespace SearchService.Domain.Entities;

public class SuggestionsLogEntity
{
    public Guid Id { get; private set; }
    public DateTime Timestamp { get; private set; }
    public string SearchQuery { get; private set; }
    
    // Relevance signals
    public Guid? ClickedResultId { get; private set; }
    
}