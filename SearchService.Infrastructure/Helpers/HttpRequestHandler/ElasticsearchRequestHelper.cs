using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using SearchService.Shared.Models;

namespace SearchService.Infrastructure.Helpers.HttpRequestHandler;

public class ElasticsearchRequestHelper(
    ILogger<ElasticsearchRequestHelper> logger,
    ElasticConfiguration elasticConfiguration) : IElasticsearchRequestHelper
{
    public async Task<JsonDocument> SearchAsync(string indexName, object payload)
    {
        using var http = new HttpClient();
        http.BaseAddress = new Uri(elasticConfiguration.ElasticUrl);

        var response = await http.PostAsJsonAsync($"{indexName}/_search", payload);
        var rawResponse = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("Elasticsearch query failed: {StatusCode} - {Response}", response.StatusCode, rawResponse);
            throw new Exception($"Elasticsearch query failed: {response.StatusCode} - {rawResponse}");
        }

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(rawResponse));
        return await JsonDocument.ParseAsync(stream);
    }
}