namespace SearchService.Application.Dto;

public class PostAccommodationClickRequest
{
    public Guid SearchId { get; set; }
    public int AccommodationRank { get; set; }
    public int Page { get; set; }
}