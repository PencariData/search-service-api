using Microsoft.EntityFrameworkCore;
using SearchService.Application.Interfaces.Repositories;
using SearchService.Domain.Entities;
using SearchService.Infrastructure.Persistence;

namespace SearchService.Infrastructure.Repositories;

public class SearchSearchLogRepository(AppDbContext dbContext) : ISearchLogRepository
{
    public async Task StoreLogAsync(SearchLogEntity entity)
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