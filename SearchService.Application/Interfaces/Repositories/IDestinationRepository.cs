namespace SearchService.Application.Interfaces.Repositories;

public interface IDestinationRepository
{
    public Task<IEnumerable<string>> GetDestinationSuggestionsAsync(string name, int limit);
}