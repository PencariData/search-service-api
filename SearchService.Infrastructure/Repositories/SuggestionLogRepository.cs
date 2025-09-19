using Microsoft.EntityFrameworkCore;
using SearchService.Application.Interfaces.Repositories;
using SearchService.Domain.Entities;
using SearchService.Infrastructure.Persistence;

namespace SearchService.Infrastructure.Repositories;

public class SuggestionLogRepository(AppDbContext dbContext) : ISuggestionLogRepository
{
    public async Task StoreLogAsync(SuggestionLogEntity entity)
    {
        await dbContext.SuggestionLogs.AddAsync(entity);
        await dbContext.SaveChangesAsync();
    }
}