using SearchService.Domain.Entities;

namespace SearchService.Application.Interfaces.Services;

public interface ILogQueueService
{
    public void Enqueue(SearchLogEntity log);
}