using System.Diagnostics;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SearchService.Application.Dto;
using SearchService.Application.Interfaces.Repositories;
using SearchService.Application.Interfaces.Services;
using SearchService.Shared.Extensions;
using SearchService.Shared.Models;
using ValidationException = FluentValidation.ValidationException;

namespace SearchService.Application.Services;

public class SearchSuggestionService(
    IAccommodationRepository accommodationRepository,
    IDestinationRepository destinationRepository,
    IValidator<GetSuggestionRequest> validator,
    IMemoryCache cache,
    CachingOptions cachingOptions,
    ILogger<SearchSuggestionService> logger) : ISearchSuggestionService
{
    public async Task<GetSuggestionResponse> GetSuggestionsAsync(GetSuggestionRequest request)
    {
        // Validate request
        await validator.ValidateAndThrowAsync(request);
        
        // Try cache
        var cacheKey = $"suggestion:{request.Query}";
        var cacheResponse = CacheLookup(cacheKey);

        if (cacheResponse != null)
        {
            return cacheResponse;
        }
        
        if (cache.TryGetValue<GetSuggestionResponse>(cacheKey, out var suggestion))
        {
            if (suggestion != null)
            {
                logger.LogInformation("Total process time : {SwElapsedMilliseconds}", 0);
                
                // Log search event
                return suggestion;
            }
        }
        
        // Get accommodation suggestion
        var accommodationSuggestions =
            await accommodationRepository.GetSuggestionsAsync(request.Query, request.Limit);
        
        // Get destination suggestion 
        var destinationSuggestions =
            await destinationRepository.GetDestinationSuggestionsAsync(request.Query, request.Limit);
        
        var response = new GetSuggestionResponse(
            accommodationSuggestions.ToList(), 
            destinationSuggestions.ToList());
        
        // Log search event
        logger.LogInformation("Total process time : {SwElapsedMilliseconds}", 0);
        
        // Cache the result
        cache.SetWithConfig(cacheKey, response, cachingOptions.SuggestionCacheDurationMinutes);
        
        return  response;
    }

    private GetSuggestionResponse? CacheLookup(
        string cacheKey)
    {
        return !cache.TryGetValue<GetSuggestionResponse>(cacheKey, out var cachedResponse) ? null : cachedResponse;
    }
}