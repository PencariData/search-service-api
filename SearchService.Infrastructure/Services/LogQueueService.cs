using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using SearchService.Application.Interfaces.Services;
using SearchService.Domain.Entities;

namespace SearchService.Infrastructure.Services;

public class LogQueueService(
    Channel<SearchLogEntity> channel,
    ILogger<LogQueueService> logger) : ILogQueueService
{
    public void Enqueue(SearchLogEntity log)
    {
        if (!channel.Writer.TryWrite(log))
        {
            logger.LogError("Failed to enqueue log for SearchId {SearchId}, Query: {Query}",
                log.SearchId, log.SearchQuery);
        }
    }
}