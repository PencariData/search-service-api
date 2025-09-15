using SearchService.Domain.Entities;

namespace SearchService.Application.Interfaces.Repositories;

public interface ISuggestionSearchLogRepository
{
    public Task StoreLogAsync(SuggestionLogEntity entity);
    public Task<SuggestionLogEntity?> GetSuggestionLogBySearchIdAsync(Guid searchId);
}