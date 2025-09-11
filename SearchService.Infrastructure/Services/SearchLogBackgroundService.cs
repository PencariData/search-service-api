using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SearchService.Application.Interfaces.Repositories;
using SearchService.Domain.Entities;

namespace SearchService.Infrastructure.Services;

public class SearchLogBackgroundService(
    Channel<SearchLogEntity> channel,
    IServiceProvider serviceProvider,
    ILogger<SearchLogBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var log in channel.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<ILogRepository>();
                await repo.StoreSearchLogAsync(log);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Error storing search log. SearchId: {SearchId}, Query: {Query}, Page: {Page}",
                    log.SearchId, log.SearchQuery, log.Page);
            }
        }
    }
}