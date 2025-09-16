using System.Diagnostics;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
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
    ISuggestionSearchLogRepository suggestionSearchLogRepository,
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
        
        // Resolve searchId
        var searchId = await ResolveSearchId(request);
        
        // Try cache
        var cacheKey = $"suggestion:{request.Query}";
        var cacheData = CacheLookup(cacheKey, request, searchId);
        if (cacheData != null)
            return cacheData;
        
        // Get suggestion
        var stopwatch = Stopwatch.StartNew();
        var accommodationSuggestions =
            (await accommodationRepository.GetSuggestionsAsync(request.Query, request.Limit)).ToList();
        var destinationSuggestions =
            (await destinationRepository.GetDestinationSuggestionsAsync(request.Query, request.Limit)).ToList();
        var elapsedMs = stopwatch.ElapsedMilliseconds;

        var response = new GetSuggestionResponse(
            Meta: new SuggestionMetaDto(
                SearchId: searchId,
                accommodationSuggestionCount: accommodationSuggestions.Count,
                destinationSuggestionCount: destinationSuggestions.Count),
            AccommodationSuggestions: accommodationSuggestions, 
            DestinationSuggestions: destinationSuggestions);

        var logContext = new LogContext(
            SearchId: searchId,
            Request: request,
            AccommodationSuggestionCount: accommodationSuggestions.Count,
            DestinationSuggestionCount: destinationSuggestions.Count,
            ElapsedMs: elapsedMs,
            FromCache: false);
        
        EnqueueLog(logContext);
        
        // Cache the result
        cache.SetWithConfig(cacheKey, response, cachingOptions.SuggestionCacheDurationMinutes);
        
        return  response;
    }

    #region Private Methods

    private GetSuggestionResponse? CacheLookup(
        string cacheKey,
        GetSuggestionRequest request,
        Guid searchId
        )
    {
        if (!cache.TryGetValue<GetSuggestionResponse>(cacheKey, out var cachedResponse))
            return null;

        var logContext = new LogContext(
            SearchId: searchId,
            Request: request,
            AccommodationSuggestionCount: cachedResponse!.Meta.accommodationSuggestionCount,
            DestinationSuggestionCount: cachedResponse.Meta.destinationSuggestionCount,
            ElapsedMs: 0,
            FromCache: true);
        
        EnqueueLog(logContext);

        return cachedResponse;
    }

    private async Task<Guid> ResolveSearchId(GetSuggestionRequest request)
    {
        var searchId = request.SearchId;

        if (request.SearchId != null)
        {
            // Assign new SearchId if the searchId have different searchQuery property
            var existingSuggestionLog = await suggestionSearchLogRepository.GetSuggestionLogBySearchIdAsync(request.SearchId.Value);
            if (existingSuggestionLog != null &&  existingSuggestionLog.Session.Query != request.Query)
            {
                searchId = Guid.NewGuid();
            }
        }
        else
        {
            searchId = Guid.NewGuid();
        }

        return searchId!.Value;
    }
    
    private void EnqueueLog(LogContext context) =>
        logQueueService.Enqueue(SuggestionLogEntity.Create(
            suggestionId: context.SearchId,
            session: new SuggestionSessionInfo(DateTime.UtcNow, context.Request.Query, context.AccommodationSuggestionCount, context.DestinationSuggestionCount),
            performance: new SuggestionPerformanceInfo(context.FromCache, context.ElapsedMs),
            interaction: null
        ));
    
    #endregion
    
    #region Data Model
    
    private record LogContext(
        Guid SearchId,
        GetSuggestionRequest Request,
        int AccommodationSuggestionCount,
        int DestinationSuggestionCount,
        bool FromCache,
        long ElapsedMs
    );
    #endregion
}