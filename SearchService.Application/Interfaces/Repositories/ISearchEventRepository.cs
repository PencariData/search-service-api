using SearchService.Domain.Events;

namespace SearchService.Application.Interfaces.Repositories;

public interface ISearchEventRepository
{
    Task AddAsync(SearchEvent domainEvent, CancellationToken cancellationToken = default);
}
