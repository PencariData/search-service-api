using System.Diagnostics;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SearchService.Application.Dto;
using SearchService.Application.Enums;
using SearchService.Application.Interfaces.Repositories;
using SearchService.Application.Interfaces.Services;
using SearchService.Domain.Entities;
using SearchService.Domain.Events;
using SearchService.Shared.Extensions;
using SearchService.Shared.Models;

namespace SearchService.Application.Services;

public class AccommodationSearchService(
    IAccommodationRepository accommodationRepository,
    ILogQueueService<SearchEvent> logQueueService,
    IValidator<GetAccommodationRequest> validator,
    IMemoryCache cache,
    CachingOptions cachingOptions,
    ILogger<AccommodationSearchService> logger)
    : IAccommodationSearchService
{
    public async Task<GetAccommodationResponse> SearchAccommodationsAsync(
        GetAccommodationRequest request)
    {
        // Validate request
        await validator.ValidateAndThrowAsync(request);
        
        // Resolve sessionId
        var sessionId = request.SessionId ?? Guid.Empty;
        if(sessionId == Guid.Empty)
            logger.LogWarning("{TimeStamp} - Request with empty sessionId", DateTime.UtcNow);

        // Try cache
        var cacheKey = $"search:{request.SearchQuery}:{request.Page}:{request.AccommodationSearchType}";
        var cacheData =  CacheLookup(cacheKey);
        if (cacheData != null)
        {
            // Store Log
            var searchPerformedFromCache = new SearchPerformed(
                sessionId: request.SessionId ?? Guid.Empty,
                searchId: request.SearchId ?? Guid.Empty,
                query: request.SearchQuery,
                page: request.Page,
                resultCount: cacheData.Accommodations.Count,
                elapsedMs: 0
            );
            
            EnqueueLog(searchPerformedFromCache);
            
            return  new GetAccommodationResponse(
                Meta: new SearchMetaDto(
                    SessionId: sessionId,
                    SearchId: request.SearchId ?? Guid.Empty,
                    Page: request.Page,
                    ResulCount: cacheData.Accommodations.Count,
                    TotalResult: cacheData.TotalResult
                ),
                Accommodations: cacheData.Accommodations
            );
        }

        // Search
        var stopwatch = Stopwatch.StartNew();
        var searchResult = await SearchAccommodation(request);
        var elapsedMs = stopwatch.ElapsedMilliseconds;

        // Map results
        var dtoList = searchResult.Results.Select(MapToDto).ToList();

        var response = new GetAccommodationResponse(
            Meta: new SearchMetaDto(
                SessionId: sessionId,
                SearchId: request.SearchId ?? Guid.Empty,
                Page: request.Page,
                ResulCount: dtoList.Count,
                TotalResult: searchResult.Total
            ),
            Accommodations: dtoList
        );
        
        // Store Log
        var searchPerformed = new SearchPerformed(
            sessionId: request.SessionId ?? Guid.Empty, 
            searchId: request.SearchId ?? Guid.Empty,
            query: request.SearchQuery,
            page: request.Page,
            resultCount: dtoList.Count,
            elapsedMs: elapsedMs
        );

        EnqueueLog(searchPerformed);

        // Cache
        var cachePayload = new CachedSearchResult(dtoList, searchResult.Total);
        cache.SetWithConfig(cacheKey, cachePayload, cachingOptions.ResultCacheDurationMinutes);

        return response;
    }
    
    #region Private Methods

    private  CachedSearchResult? CacheLookup(
        string cacheKey)
    {
        return !cache.TryGetValue<CachedSearchResult>(cacheKey, out var cachedResponse) ? null : cachedResponse;
    }
    
    private async Task<SearchResultDto<AccommodationEntity>> SearchAccommodation(GetAccommodationRequest request)
    {
        var searchResult = request.AccommodationSearchType switch
        {
            AccommodationSearchType.FreeSearch => await accommodationRepository.GetByMultipleFieldAsync(
                ["destinationName.ngram", "name.ngram"],
                request.SearchQuery,
                request.Page,
                request.Limit),
            
            AccommodationSearchType.ByDestination => await accommodationRepository.GetByFieldAsync(
                "destinationName",
                request.SearchQuery,
                request.Page,
                request.Limit),
            
            AccommodationSearchType.ByName => await accommodationRepository.GetByFieldAsync(
                "name",
                request.SearchQuery,
                request.Page,
                request.Limit),
            
            _ => throw new InvalidOperationException("SearchType undefined")
        };

        return searchResult;
    }
    
    private static AccommodationDto MapToDto(AccommodationEntity entity) =>
        new(entity.Id, entity.Name, entity.FullDestination, entity.AccommodationType, entity.Coordinate);

    private void EnqueueLog(SearchEvent searchLog) =>
        logQueueService.Enqueue(searchLog);
    
    private record CachedSearchResult(List<AccommodationDto> Accommodations, int TotalResult);
    
    #endregion
}