using System.Text.Json;

namespace SearchService.Infrastructure.Helpers.HttpRequestHandler;

public interface IElasticsearchRequestHelper
{
    public Task<JsonDocument> SearchAsync(string indexName, object payload);
}