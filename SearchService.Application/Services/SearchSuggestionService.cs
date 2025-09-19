using System.Diagnostics;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SearchService.Application.Dto;
using SearchService.Application.Interfaces.Repositories;
using SearchService.Application.Interfaces.Services;
using SearchService.Domain.Entities;
using SearchService.Domain.ValueObjects;
using SearchService.Shared.Extensions;
using SearchService.Shared.Models;

namespace SearchService.Application.Services;

public class SearchSuggestionService(
    IAccommodationRepository accommodationRepository,
    ISuggestionLogRepository suggestionLogRepository,
    ILogger<SearchSuggestionService> logger,
    ILogQueueService<SuggestionLogEntity> logQueueService,
    IDestinationRepository destinationRepository,
    IValidator<GetSuggestionRequest> validator,
    IMemoryCache cache,
    CachingOptions cachingOptions) : ISearchSuggestionService
{
    public async Task<GetSuggestionResponse> GetSuggestionsAsync(GetSuggestionRequest request)
    {
        // Validate request
        await validator.ValidateAndThrowAsync(request);
        
        var sessionId = request.SessionId ?? Guid.Empty;
        if(sessionId == Guid.Empty)
            logger.LogWarning("{TimeStamp} - Request with empty sessionId", DateTime.UtcNow);
        
        // Try cache
        var cacheKey = $"suggestion:{request.Query}";
        var cacheData = CacheLookup(cacheKey);
        if (cacheData != null)
        {
            var suggestionLogFromCache = SuggestionLogEntity.Create(
                sessionId: sessionId, 
                query: request.Query, 
                accommodationSuggestionCount: cacheData.AccommodationSuggestions.Count, 
                destinationSuggestionCount: cacheData.DestinationSuggestions.Count,
                elapsedMs: 0);
            
            EnqueueLog(suggestionLogFromCache);
            
            return cacheData;
        }
        
        // Get suggestion
        var stopwatch = Stopwatch.StartNew();
        var accommodationSuggestions = (await accommodationRepository.GetSuggestionsAsync(request.Query, request.Limit)).ToList();
        var destinationSuggestions = (await destinationRepository.GetDestinationSuggestionsAsync(request.Query, request.Limit)).ToList();
        var elapsedMs = stopwatch.ElapsedMilliseconds;
        
        // Map results
        var response = new GetSuggestionResponse(
            Meta: new SuggestionMetaDto(
                SessionId: sessionId,
                accommodationSuggestionCount: accommodationSuggestions.Count,
                destinationSuggestionCount: destinationSuggestions.Count
            ),
            AccommodationSuggestions: accommodationSuggestions, 
            DestinationSuggestions: destinationSuggestions
        );
        
        // Store Log
        var searchLogFromSearch = SuggestionLogEntity.Create(
            sessionId: sessionId,
            query: request.Query,
            accommodationSuggestionCount: accommodationSuggestions.Count,
            destinationSuggestionCount: destinationSuggestions.Count,
            elapsedMs: elapsedMs);
        
        EnqueueLog(searchLogFromSearch);
        
        // Cache
        cache.SetWithConfig(cacheKey, response, cachingOptions.SuggestionCacheDurationMinutes);
        
        return  response;
    }

    #region Private Methods

    private GetSuggestionResponse? CacheLookup(string cacheKey)
    {
        return !cache.TryGetValue<GetSuggestionResponse>(cacheKey, out var cachedResponse) ? null : cachedResponse;
    }
    
    private void EnqueueLog(SuggestionLogEntity log) =>
        logQueueService.Enqueue(log);
    
    #endregion
}