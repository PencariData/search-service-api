using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using SearchService.Application.Interfaces.Services;
using SearchService.Domain.Entities;

namespace SearchService.Infrastructure.Services;

public class LogQueueService<T>(
    Channel<T> channel,
    ILogger<LogQueueService<T>> logger) : ILogQueueService<T>
{
    public void Enqueue(T log)
    {
        if (!channel.Writer.TryWrite(log))
        {
            logger.LogError("Failed to enqueue log: {@Log}", log);
        }
    }
}