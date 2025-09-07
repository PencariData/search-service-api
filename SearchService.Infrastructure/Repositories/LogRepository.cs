using Microsoft.EntityFrameworkCore;
using SearchService.Application.Interfaces.Repositories;
using SearchService.Domain.Entities;
using SearchService.Infrastructure.Persistence;

namespace SearchService.Infrastructure.Repositories;

public class LogRepository(AppDbContext dbContext) : ILogRepository
{
    public async Task StoreSearchLogAsync(SearchLogEntity entity)
    {
        await dbContext.SearchLogs.AddAsync(entity);
        await dbContext.SaveChangesAsync();
    }

    public async Task<SearchLogEntity?> GetSearchLogBySearchIdAsync(Guid searchId)
    {
        return await dbContext.SearchLogs
            .FirstOrDefaultAsync(x => x.SearchId == searchId);
    }
}