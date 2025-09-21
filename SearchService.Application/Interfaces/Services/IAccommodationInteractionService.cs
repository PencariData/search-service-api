using SearchService.Application.Dto;

namespace SearchService.Application.Interfaces.Services;

public interface IAccommodationInteractionService
{
    public Task RegisterClickAsync(PostAccommodationClickRequest request);
}