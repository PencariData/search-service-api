using SearchService.Domain.Entities;

namespace SearchService.Application.Interfaces.Repositories;

public interface ISearchLogRepository
{
    public Task StoreLogAsync(SearchLogEntity entity);
    public Task<SearchLogEntity?> GetSearchLogBySearchIdAsync(Guid searchId);
}