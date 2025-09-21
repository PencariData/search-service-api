using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SearchService.Application.Interfaces.Repositories;
using SearchService.Domain.Events;

namespace SearchService.Infrastructure.Services;

public class BackgroundLogService<TEvent, TRepository>(
    Channel<TEvent> channel,
    IServiceProvider services,
    ILogger<BackgroundLogService<TEvent, TRepository>> logger)
    : BackgroundService
    where TEvent : SearchEvent
    where TRepository : class, ISearchEventRepository
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var log in channel.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                // resolve repository per event to keep DbContext lifetime scoped
                using var scope = services.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<TRepository>();

                await repo.AddAsync(log, stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to persist log event {@Log}", log);
            }
        }
    }
}