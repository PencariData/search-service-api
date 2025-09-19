using SearchService.Domain.Entities;

namespace SearchService.Application.Interfaces.Repositories;

public interface ISuggestionLogRepository
{
    public Task StoreLogAsync(SuggestionLogEntity entity);
}