using System.Text.Json;
using Microsoft.Extensions.Logging;
using SearchService.Application.Dto;
using SearchService.Application.Interfaces.Repositories;
using SearchService.Domain.Entities;
using SearchService.Infrastructure.Helpers.HttpRequestHandler;
using SearchService.Infrastructure.Helpers.Mapping;
using SearchService.Shared.Models;

namespace SearchService.Infrastructure.Repositories;

public class AccommodationRepository(
    IElasticsearchRequestHelper elasticsearchRequestHelper,
    ILogger<AccommodationRepository> logger,
    ElasticConfiguration elasticConfiguration) : IAccommodationRepository
{
    /// <summary>
    /// Get accommodation documents with (name/destination) fields as filter
    /// </summary>
    /// <param name="field"></param>
    /// <param name="query"></param>
    /// <param name="limit"></param>
    /// <returns></returns>
    public async Task<SearchResultDto<AccommodationEntity>> GetByFieldAsync(string field, string query, int page, int limit)
    {   var payload = new
        {
            from = page * limit,
            size = limit,
            query = new
            {
                match = new Dictionary<string, object>
                {
                    [field] = new { query = query, fuzziness = "AUTO" }
                }
            }
        };

        var jsonDoc = await elasticsearchRequestHelper.SearchAsync(elasticConfiguration.AccommodationIndex, payload);

        if (!jsonDoc.RootElement.TryGetProperty("hits", out var hitsElement))
        {
            logger.LogWarning("[Search] No matches found for {Field}: '{Query}'", field, query);
            return new SearchResultDto<AccommodationEntity>([], 0);
        }

        var total = hitsElement.GetProperty("total").GetProperty("value").GetInt32();

        if (!hitsElement.TryGetProperty("hits", out var hitsArray) || hitsArray.GetArrayLength() == 0)
        {
            return new SearchResultDto<AccommodationEntity>([], total);
        }

        var results = hitsArray
            .EnumerateArray()
            .Where(hit => hit.TryGetProperty("_source", out _))
            .Select(hit =>
            {
                using var sourceDoc = JsonDocument.Parse(hit.GetProperty("_source").GetRawText());
                return JsonToAccommodationMapper.Map(sourceDoc);
            })
            .ToList();

        return new SearchResultDto<AccommodationEntity>(results, total);    
    }
    
    /// <summary>
    /// Get accommodation documents using multiple field as filter
    /// </summary>
    /// <param name="fields"></param>
    /// <param name="query"></param>
    /// <param name="limit"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<SearchResultDto<AccommodationEntity>> GetByMultipleFieldAsync(List<string> fields, string query, int page, int limit)
    {
        
        var payload = new
        {
            from = page * limit,
            size = limit,
            query = new
            {
                multi_match = new
                {
                    query,
                    fields,
                    fuzziness = "AUTO"
                }
            }
        };

        var jsonDoc = await elasticsearchRequestHelper.SearchAsync(elasticConfiguration.AccommodationIndex, payload);

        if (!jsonDoc.RootElement.TryGetProperty("hits", out var hitsElement))
        {
            logger.LogWarning("[Search] No matches found for {Field}: '{Query}'", fields, query);
            return new SearchResultDto<AccommodationEntity>([], 0);
        }

        var total = hitsElement.GetProperty("total").GetProperty("value").GetInt32();

        if (!hitsElement.TryGetProperty("hits", out var hitsArray) || hitsArray.GetArrayLength() == 0)
        {
            return new SearchResultDto<AccommodationEntity>([], total);
        }

        var results = hitsArray
            .EnumerateArray()
            .Where(hit => hit.TryGetProperty("_source", out _))
            .Select(hit =>
            {
                using var sourceDoc = JsonDocument.Parse(hit.GetProperty("_source").GetRawText());
                return JsonToAccommodationMapper.Map(sourceDoc);
            })
            .ToList();

        return new SearchResultDto<AccommodationEntity>(results, total);
    }

    /// <summary>
    /// Get accommodation suggestions based on name property
    /// </summary>
    /// <param name="name"></param>
    /// <param name="limit"></param>
    /// <returns></returns>
    public async Task<IEnumerable<string>> GetSuggestionsAsync(string name, int limit)
    {
        var payload = new
        {
            size = 0,
            suggest = new
            {
                accommodation_suggest = new
                {
                    prefix = name,
                    completion = new
                    {
                        field = "name_suggest",
                        size = limit,
                        fuzzy = new
                        {
                            fuzziness = 2
                        }
                    }
                }
            }
        };

        var jsonDoc = await elasticsearchRequestHelper.SearchAsync(elasticConfiguration.AccommodationIndex, payload);

        if (!jsonDoc.RootElement.TryGetProperty("suggest", out var suggestElement) ||
            !suggestElement.TryGetProperty("accommodation_suggest", out var hotelSuggestArray) ||
            hotelSuggestArray.GetArrayLength() == 0)
        {
            logger.LogWarning("[Suggestions] No suggestions found for name: '{Name}'", name);
            return Array.Empty<string>();
        }

        var firstSuggest = hotelSuggestArray[0];

        if (!firstSuggest.TryGetProperty("options", out var optionsArray))
        {
            logger.LogWarning("[Suggestions] No 'options' found in Elasticsearch response for name: '{Name}'", name);
            return Array.Empty<string>();
        }
        
        // Log the number of options found
        logger.LogInformation("Found {Count} options in Elasticsearch response", optionsArray.GetArrayLength());


        var suggestions = optionsArray
            .EnumerateArray()
            .Select(opt => opt.GetProperty("_source").GetProperty("name").GetString() ?? string.Empty)
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToList();

        return suggestions;
    }

}