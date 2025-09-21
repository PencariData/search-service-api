namespace SearchService.Infrastructure.Entities;

public class SearchEventEntity
{
    public Guid EventId { get; set; }
    public Guid SessionId { get; set; }
    public Guid? SearchId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty; // stored as JSON
    public DateTime OccurredAt { get; set; }
}
