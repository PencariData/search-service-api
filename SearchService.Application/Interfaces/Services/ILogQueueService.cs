using SearchService.Domain.Entities;

namespace SearchService.Application.Interfaces.Services;

public interface ILogQueueService<in T>
{
    public void Enqueue(T log);
}