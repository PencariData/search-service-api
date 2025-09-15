using Microsoft.EntityFrameworkCore;
using SearchService.Application.Interfaces.Repositories;
using SearchService.Domain.Entities;
using SearchService.Infrastructure.Persistence;

namespace SearchService.Infrastructure.Repositories;

public class SuggestionSearchSearchLogRepository(AppDbContext dbContext) : ISuggestionSearchLogRepository
{
    public async Task StoreLogAsync(SuggestionLogEntity entity)
    {
        await dbContext.SuggestionLogs.AddAsync(entity);
        await dbContext.SaveChangesAsync();
    }

    public async Task<SuggestionLogEntity?> GetSuggestionLogBySearchIdAsync(Guid searchId)
    {
        return await dbContext.SuggestionLogs
            .FirstOrDefaultAsync(x => x.SearchId == searchId);
    }
}