using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SearchService.Infrastructure.Services;

public class BackgroundLogService<TLog, TRepo>(
    Channel<TLog> channel,
    IServiceProvider serviceProvider,
    ILogger<BackgroundLogService<TLog, TRepo>> logger)
: BackgroundService
where TRepo : class
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var log in channel.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<TRepo>();

                // Assuming repo has StoreLogAsync(TLog log)
                var method = typeof(TRepo).GetMethod("StoreLogAsync");
                if (method != null)
                {
                    await (Task)method.Invoke(repo, new object[] { log })!;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error storing log {@Log}", log);
            }
        }
    }
}