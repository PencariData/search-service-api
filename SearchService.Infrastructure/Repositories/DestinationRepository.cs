using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using SearchService.Application.Interfaces.Repositories;
using SearchService.Shared.Models;

namespace SearchService.Infrastructure.Repositories;

public class DestinationRepository(
    ILogger<DestinationRepository> logger,
    ElasticConfiguration elasticConfiguration) : IDestinationRepository
{
    public async Task<IEnumerable<string>> GetDestinationSuggestionsAsync(string query, int limit)
    {
         logger.LogDebug("[Suggestions] Request started — name: '{Name}', limit: {Limit}", query, limit);

        var payload = new
        {
            size = 0,
            suggest = new
            {
                destination_suggest = new
                {
                    prefix = query,
                    completion = new
                    {
                        field = "name",
                        skip_duplicates = true,
                        size = limit
                    }
                }
            }
        };

        logger.LogDebug("[Suggestions] Elasticsearch request payload:\n{Payload}",
            JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true }));

        using var http = new HttpClient();
        http.BaseAddress = new Uri(elasticConfiguration.ElasticUrl);

        var response = await http.PostAsJsonAsync($"{elasticConfiguration.DestinationIndex}/_search", payload);

        var rawResponse = await response.Content.ReadAsStringAsync();
        
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Elasticsearch query failed: {response.StatusCode} - {rawResponse}");
        }

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(rawResponse));
        using var jsonDoc = await JsonDocument.ParseAsync(stream);

        if (!jsonDoc.RootElement.TryGetProperty("suggest", out var suggestElement) ||
            !suggestElement.TryGetProperty("destination_suggest", out var destinationSuggestArray) ||
            destinationSuggestArray.GetArrayLength() == 0)
        {
            logger.LogWarning("[Suggestions] No suggestions found for name: '{Name}'", query);
            return [];
        }

        var firstSuggest = destinationSuggestArray[0];
        if (!firstSuggest.TryGetProperty("options", out var optionsArray))
        {
            logger.LogWarning("[Suggestions] No 'options' found in Elasticsearch response for name: '{Name}'", query);
            return [];
        }

        var suggestions = optionsArray
            .EnumerateArray()
            .Select(opt => opt.GetProperty("text").GetString() ?? string.Empty)
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Distinct()
            .ToList();

        logger.LogDebug("[Suggestions] Extracted results: {@Suggestions}", suggestions);

        return suggestions;
    }
}