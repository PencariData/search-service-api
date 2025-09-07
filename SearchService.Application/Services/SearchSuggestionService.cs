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
    IValidator<GetSuggestionRequest> getSuggestionValidator,
    IMemoryCache cache,
    CachingOptions cachingOptions,
    ILogger<SearchSuggestionService> logger) : ISearchSuggestionService
{
    public async Task<GetSuggestionResponse> GetSuggestionsAsync(GetSuggestionRequest request)
    {
        var validationResult = await getSuggestionValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
        
        var sw = Stopwatch.StartNew();

        var cacheKey = $"suggestion:{request.Query}";

        if (cache.TryGetValue<GetSuggestionResponse>(cacheKey, out var suggestion))
        {
            if (suggestion != null)
            {
                sw.Stop();
                logger.LogInformation("Total process time : {SwElapsedMilliseconds}", sw.ElapsedMilliseconds);
                
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
        sw.Stop(); 
        logger.LogInformation("Total process time : {SwElapsedMilliseconds}", sw.ElapsedMilliseconds);
        
        // Cache the result
        cache.SetWithConfig(cacheKey, response, cachingOptions.SuggestionCacheDurationMinutes);
        
        return  response;
    }
}