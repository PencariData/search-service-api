using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using SearchService.Application.Interfaces.Services;
using SearchService.Domain.Entities;

namespace SearchService.Infrastructure.Services;

public class LogQueueService(
    Channel<SearchLogEntity> channel) : ILogQueueService
{
    public void Enqueue(SearchLogEntity log)
    {
        if (!channel.Writer.TryWrite(log))
        {
            
        }
    }
}