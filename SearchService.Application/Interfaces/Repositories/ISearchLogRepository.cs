using SearchService.Domain.Entities;

namespace SearchService.Application.Interfaces.Repositories;

public interface ISearchLogRepository
{
    Task StoreLogAsync(SearchLogEntity entity);
    Task<SearchLogEntity?> GetSearchLogBySearchIdAsync(Guid searchId);
}