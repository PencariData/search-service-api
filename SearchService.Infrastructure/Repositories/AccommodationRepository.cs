using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.Extensions.Logging;
using SearchService.Application.Interfaces.Repositories;
using SearchService.Domain.Entities;
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
        var searchResponse = await elasticClient.SearchAsync<JsonDocument>(s => s
            .Indices(elasticConfiguration.AccommodationIndex)
            .Size(limit)
            .Query(q => q
                .Bool(b => b
                    .Should(
                        sh => sh.Term(t => t.Field("name.keyword").Value(name).Boost(5)),
                        sh => sh.MultiMatch(m => m
                            .Query(name)
                            .Fields(new[] { "name", "name.keyword^3" })
                            .Type(TextQueryType.BestFields)
                            .Fuzziness(new Fuzziness("AUTO"))
                        )
                    )
                )
            )
            .Sort(srt => srt.Score(sc => sc.Order(SortOrder.Desc)))
        );

        if (searchResponse.IsValidResponse)
        {
            return searchResponse.Documents.Select(JsonToAccommodationMapper.Map);
        }
        
        if (!string.IsNullOrWhiteSpace(searchResponse.ApiCallDetails.OriginalException.Message))
        {
            throw new Exception(
                "Failed to connect to Elasticsearch",
                searchResponse.ApiCallDetails.OriginalException.InnerException 
            );
        }

        if (!string.IsNullOrWhiteSpace(searchResponse.ElasticsearchServerError.Error.Reason))
        {
            throw new Exception(
                $"Elasticsearch query failed: {searchResponse.ElasticsearchServerError.Error.Reason}"
            );
        }

        throw new Exception("Unknown Elasticsearch error occurred");
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
        var searchResponse = await elasticClient.SearchAsync<JsonDocument>(s => s
            .Indices(elasticConfiguration.AccommodationIndex)
            .Size(limit)
            .Query(q => q
                .MultiMatch(m => m
                    .Query(destination)
                    .Fields(new[]
                    {
                        "country",
                        "administrationLevel1",
                        "administrationLevel2",
                        "fullDestination^2" // boost full destination
                    })
                    .Type(TextQueryType.BestFields)
                    .Fuzziness(new Fuzziness("AUTO"))
                )
            )
            .Sort(srt => srt.Score(sc => sc.Order(SortOrder.Desc)))
        );

        if (!searchResponse.IsValidResponse)
            throw new Exception($"Elasticsearch query failed: {searchResponse.ElasticsearchServerError.Error.Reason}");

        return searchResponse.Documents.Select(JsonToAccommodationMapper.Map);
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