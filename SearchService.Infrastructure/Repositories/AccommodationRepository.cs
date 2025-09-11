using System.Text.Json;
using Microsoft.Extensions.Logging;
using SearchService.Application.Dto;
using SearchService.Application.Interfaces.Repositories;
using SearchService.Domain.Entities;
using SearchService.Infrastructure.Helpers.HttpRequestHandler;
using SearchService.Infrastructure.Helpers.Mapping;
using SearchService.Infrastructure.Helpers.QueryBuilding;
using SearchService.Shared.Constants;
using SearchService.Shared.Models;

namespace SearchService.Infrastructure.Repositories;

public class AccommodationRepository(
    IElasticsearchRequestHelper elasticsearchRequestHelper,
    ILogger<AccommodationRepository> logger,
    ElasticConfiguration elasticConfiguration) : IAccommodationRepository
{
    /// <summary>
    /// Get accommodation documents with a single field as filter
    /// </summary>
    /// <param name="field">The field to search in</param>
    /// <param name="query">The search query</param>
    /// <param name="page">Page number (0-based)</param>
    /// <param name="limit">Number of results per page</param>
    /// <returns>Search results with accommodations and total count</returns>
    public async Task<SearchResultDto<AccommodationEntity>> GetByFieldAsync(string field, string query, int page, int limit)
    {
        var payload = new ElasticsearchQueryBuilder()
            .WithPagination(page, limit)
            .WithMatch(field, query)
            .Build();

        return await ExecuteSearchAsync(payload, query, page, field);
    }
    
    /// <summary>
    /// Get accommodation documents using multiple fields as filter
    /// </summary>
    /// <param name="fields">The fields to search in</param>
    /// <param name="query">The search query</param>
    /// <param name="page">Page number (0-based)</param>
    /// <param name="limit">Number of results per page</param>
    /// <returns>Search results with accommodations and total count</returns>
    public async Task<SearchResultDto<AccommodationEntity>> GetByMultipleFieldAsync(List<string> fields, string query, int page, int limit)
    {
        var payload = new ElasticsearchQueryBuilder()
            .WithPagination(page, limit)
            .WithMultiMatch(fields, query)
            .Build();

        return await ExecuteSearchAsync(payload, query, page, string.Join(", ", fields));
    }

    /// <summary>
    /// Get accommodation suggestions based on name property
    /// </summary>
    /// <param name="name">The name prefix to get suggestions for</param>
    /// <param name="limit">Maximum number of suggestions to return</param>
    /// <returns>List of suggested accommodation names</returns>
    public async Task<IEnumerable<string>> GetSuggestionsAsync(string name, int limit)
    {
        var payload = new ElasticsearchQueryBuilder()
            .WithSize(0)
            .WithSuggestion(DataConstants.AccommodationSuggestKey, name, DataConstants.NameSuggestField, limit)
            .Build();

        try
        {
            var jsonDoc = await elasticsearchRequestHelper.SearchAsync(elasticConfiguration.AccommodationIndex, payload);
            return ExtractSuggestions(jsonDoc, name);
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "Failed to parse Elasticsearch suggestions response for name: '{Name}'", name);
            return [];
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting suggestions for name: '{Name}'", name);
            return [];
        }
    }
    
    #region Private Methods

    private async Task<SearchResultDto<AccommodationEntity>> ExecuteSearchAsync(
        object payload,
        string queryInfo,
        int page,
        object fieldInfo)
    {
        try
        {
            var accommodationSearchResult = await SearchAccommodationAsync(payload);

            if (accommodationSearchResult.accommodations.Count == 0)
            {
                logger.LogWarning("No results found for query: '{Query}' | Page: {Page} | Fields: {Fields}",
                    queryInfo, page, fieldInfo);
            }
            else
            {
                logger.LogInformation("Found {Count} results for query: '{Query}' | Page: {Page}",
                    accommodationSearchResult.accommodations.Count, queryInfo, page);
            }

            return new SearchResultDto<AccommodationEntity>(
                accommodationSearchResult.accommodations,
                accommodationSearchResult.total);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error executing search for query: '{Query}' | Page: {Page} | Fields: {Fields}",
                queryInfo, page, fieldInfo);
            return new SearchResultDto<AccommodationEntity>([], 0);
        }
    }

    private async Task<(List<AccommodationEntity> accommodations, int total)> SearchAccommodationAsync(object payload)
    {
        try
        {
            var jsonDoc = await elasticsearchRequestHelper.SearchAsync(elasticConfiguration.AccommodationIndex, payload);

            if (!jsonDoc.RootElement.TryGetProperty("hits", out var hitsElement))
            {
                logger.LogWarning("Elasticsearch response missing 'hits' property");
                return ([], 0);
            }

            var total = ExtractTotalHits(hitsElement);

            if (!hitsElement.TryGetProperty("hits", out var hitsArray) || hitsArray.GetArrayLength() == 0)
            {
                return ([], total);
            }

            var accommodations = ParseAccommodationHits(hitsArray);
            return (accommodations, total);
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "Failed to parse Elasticsearch search response");
            return ([], 0);
        }
    }

    private static int ExtractTotalHits(JsonElement hitsElement)
    {
        if (hitsElement.TryGetProperty("total", out var totalElement))
        {
            // Handle both number and object format for total
            if (totalElement.ValueKind == JsonValueKind.Number)
            {
                return totalElement.GetInt32();
            }
            if (totalElement.TryGetProperty("value", out var valueElement))
            {
                return valueElement.GetInt32();
            }
        }
        
        return 0;
    }

    private List<AccommodationEntity> ParseAccommodationHits(JsonElement hitsArray)
    {
        var accommodations = new List<AccommodationEntity>();

        foreach (var hit in hitsArray.EnumerateArray())
        {
            if (!hit.TryGetProperty("_source", out var sourceElement))
            {
                logger.LogWarning("Hit missing '_source' property, skipping");
                continue;
            }

            try
            {
                using var sourceDoc = JsonDocument.Parse(sourceElement.GetRawText());
                var accommodation = JsonToAccommodationMapper.Map(sourceDoc);
                accommodations.Add(accommodation);
            }
            catch (JsonException ex)
            {
                logger.LogWarning(ex, "Failed to parse accommodation from hit, skipping");
            }
        }

        return accommodations;
    }

    private IEnumerable<string> ExtractSuggestions(JsonDocument jsonDoc, string searchName)
    {
        if (!TryGetSuggestionsArray(jsonDoc, out var optionsArray))
        {
            logger.LogWarning("No suggestions found for name: '{Name}'", searchName);
            return [];
        }

        logger.LogInformation("Found {Count} suggestion options for name: '{Name}'",
            optionsArray.GetArrayLength(), searchName);

        return optionsArray
            .EnumerateArray()
            .Select(ExtractSuggestionText)
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToList();
    }

    private static bool TryGetSuggestionsArray(JsonDocument jsonDoc, out JsonElement optionsArray)
    {
        optionsArray = default;

        return jsonDoc.RootElement.TryGetProperty("suggest", out var suggestElement) &&
               suggestElement.TryGetProperty(DataConstants.AccommodationSuggestKey, out var suggestArray) &&
               suggestArray.GetArrayLength() > 0 &&
               suggestArray[0].TryGetProperty("options", out optionsArray);
    }

    private static string ExtractSuggestionText(JsonElement option)
    {
        try
        {
            return option.GetProperty("_source").GetProperty("name").GetString() ?? string.Empty;
        }
        catch (KeyNotFoundException)
        {
            return string.Empty;
        }
    }

    #endregion
}