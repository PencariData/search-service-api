using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.Extensions.Logging;
using SearchService.Application.Interfaces.Repositories;
using SearchService.Domain.Entities;
using SearchService.Domain.Enums;
using SearchService.Domain.ValueObjects;
using SearchService.Infrastructure.Helpers.Mapping;
using SearchService.Shared.Models;

namespace SearchService.Infrastructure.Repositories;

public class AccommodationRepository(
    ElasticsearchClient elasticClient,
    ILogger<AccommodationRepository> logger,
    ElasticConfiguration elasticConfiguration) : IAccommodationRepository
{
    /// <summary>
    /// Get accommodation documents that have match name property
    /// </summary>
    /// <param name="name"></param>
    /// <param name="limit"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<IEnumerable<AccommodationEntity>> GetByNameAsync(string name, int limit)
    {
        var payload = new
        {
            size = limit,
            query = new
            {
                match = new
                {
                    name = new
                    {
                        query = name,
                        fuzziness = "AUTO"
                    }
                }
            }
        };

        using var http = new HttpClient();
        http.BaseAddress = new Uri(elasticConfiguration.ElasticUrl);

        var response = await http.PostAsJsonAsync($"{elasticConfiguration.AccommodationIndex}/_search", payload);
        var rawResponse = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Elasticsearch query failed: {response.StatusCode} - {rawResponse}");
        }

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(rawResponse));
        using var jsonDoc = await JsonDocument.ParseAsync(stream);

        // Navigate to hits.hits array
        if (!jsonDoc.RootElement.TryGetProperty("hits", out var hitsElement) ||
            !hitsElement.TryGetProperty("hits", out var hitsArray) ||
            hitsArray.GetArrayLength() == 0)
        {
            logger.LogWarning("[Search] No matches found for name: '{Name}'", name);
            return [];
        }

        var results = new List<AccommodationEntity>();

        foreach (var hit in hitsArray.EnumerateArray())
        {
            if (!hit.TryGetProperty("_source", out var source))
                continue;

            // Wrap the _source JSON element in a JsonDocument
            using var sourceDoc = JsonDocument.Parse(source.GetRawText());
            var entity = JsonToAccommodationMapper.Map(sourceDoc);
            results.Add(entity);
        }

        return results;
    }
    
    /// <summary>
    /// Get accommodation documents that have match destination properties
    /// </summary>
    /// <param name="destination"></param>
    /// <param name="limit"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<IEnumerable<AccommodationEntity>> GetByDestinationAsync(string destination, int limit)
    {
        var payload = new
        {
            size = limit,
            query = new
            {
                match = new
                {
                    destinationName = new
                    {
                        query = destination,
                        fuzziness = "AUTO"
                    }
                }
            }
        };

        using var http = new HttpClient();
        http.BaseAddress = new Uri(elasticConfiguration.ElasticUrl);

        var response = await http.PostAsJsonAsync($"{elasticConfiguration.AccommodationIndex}/_search", payload);
        var rawResponse = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Elasticsearch query failed: {response.StatusCode} - {rawResponse}");
        }

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(rawResponse));
        using var jsonDoc = await JsonDocument.ParseAsync(stream);

        // Navigate to hits.hits array
        if (!jsonDoc.RootElement.TryGetProperty("hits", out var hitsElement) ||
            !hitsElement.TryGetProperty("hits", out var hitsArray) ||
            hitsArray.GetArrayLength() == 0)
        {
            logger.LogWarning("[Search] No matches found for destination: '{Destination}'", destination);
            return [];
        }

        var results = new List<AccommodationEntity>();

        foreach (var hit in hitsArray.EnumerateArray())
        {
            if (!hit.TryGetProperty("_source", out var source))
                continue;

            // Wrap the _source JSON element in a JsonDocument
            using var sourceDoc = JsonDocument.Parse(source.GetRawText());
            var entity = JsonToAccommodationMapper.Map(sourceDoc);
            results.Add(entity);
        }

        return results;
    }
    
    /// <summary>
    /// Get accommodation suggestions by its name
    /// </summary>
    /// <param name="name"></param>
    /// <param name="limit"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<IEnumerable<string>> GetSuggestionsByNameAsync(string name, int limit)
    {
        var payload = new
        {
            size = 0,
            suggest = new
            {
                hotel_suggest = new
                {
                    prefix = name,
                    completion = new
                    {
                        field = "name.suggest",
                        skip_duplicates = true,
                        size = limit
                    }
                }
            }
        };

        using var http = new HttpClient();
        http.BaseAddress = new Uri(elasticConfiguration.ElasticUrl);

        var response = await http.PostAsJsonAsync($"{elasticConfiguration.AccommodationIndex}/_search", payload);

        var rawResponse = await response.Content.ReadAsStringAsync();
        
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Elasticsearch query failed: {response.StatusCode} - {rawResponse}");
        }

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(rawResponse));
        using var jsonDoc = await JsonDocument.ParseAsync(stream);

        if (!jsonDoc.RootElement.TryGetProperty("suggest", out var suggestElement) ||
            !suggestElement.TryGetProperty("hotel_suggest", out var hotelSuggestArray) ||
            hotelSuggestArray.GetArrayLength() == 0)
        {
            logger.LogWarning("[Suggestions] No suggestions found for name: '{Name}'", name);
            return [];
        }

        var firstSuggest = hotelSuggestArray[0];
        if (!firstSuggest.TryGetProperty("options", out var optionsArray))
        {
            logger.LogWarning("[Suggestions] No 'options' found in Elasticsearch response for name: '{Name}'", name);
            return [];
        }

        var suggestions = optionsArray
            .EnumerateArray()
            .Select(opt => opt.GetProperty("text").GetString() ?? string.Empty)
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Distinct()
            .ToList();

        return suggestions;
    }
}