using SearchService.Domain.Entities;

namespace SearchService.Application.Interfaces.Repositories;

public interface ILogRepository
{
    public Task StoreSearchLogAsync(SearchLogEntity entity);
    public Task<SearchLogEntity?> GetSearchLogBySearchIdAsync(Guid searchId);
}