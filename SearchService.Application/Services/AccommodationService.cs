using System.Diagnostics;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using SearchService.Application.Dto;
using SearchService.Application.Enums;
using SearchService.Application.Interfaces.Repositories;
using SearchService.Application.Interfaces.Services;
using SearchService.Domain.Entities;
using SearchService.Domain.Enums;
using SearchService.Shared.Extensions;
using SearchService.Shared.Models;

namespace SearchService.Application.Services;

public class AccommodationService(
    IAccommodationRepository accommodationRepository,
    ILogRepository logRepository,
    ILogQueueService logQueueService,
    IValidator<GetAccommodationRequest> validator,
    IMemoryCache cache,
    CachingOptions cachingOptions)
    : IAccommodationService
{
    public async Task<GetAccommodationResponse> SearchAccommodationsAsync(GetAccommodationRequest request)
    {
        // Validate request
        await validator.ValidateAndThrowAsync(request);

        // Determine log validity
        var (logValidity, invalidReason) = DeriveLogContextValidity(request);

        // Resolve searchId
        var searchId = await ResolveSearchId(request);

        // Try cache
        var cacheKey = $"search:{request.SearchQuery}:{request.Page}:{request.AccommodationSearchType}";
        var cacheData = CacheLookup(cacheKey, request, logValidity, invalidReason, searchId);
        if (cacheData != null)
            return cacheData;

        // Search
        var stopwatch = Stopwatch.StartNew();
        var searchResult = await SearchAccommodation(request);
        var elapsedMs = stopwatch.ElapsedMilliseconds;

        // Map results
        var dtoList = searchResult.Results.Select(MapToDto).ToList();

        var response = new GetAccommodationResponse(
            Meta: new SearchMetaDto(
                SearchId: searchId,
                Page: request.Page,
                ResulCount: dtoList.Count,
                TotalResult: searchResult.Total
            ),
            Accommodations: dtoList
        );

        // Log
        var logContext = new LogContext(
            SearchId: searchId,
            Request: request,
            Page: request.Page,
            ResultCount: dtoList.Count,
            TotalResultCount: searchResult.Total,
            FromCache: false,
            ElapsedMs: elapsedMs,
            Validity: logValidity,
            InvalidReason: invalidReason
        );

        EnqueueLog(logContext); // Log search using search

        // Cache
        cache.SetWithConfig(cacheKey, response, cachingOptions.ResultCacheDurationMinutes);

        return response;
    }
    
    #region Private Methods
    
    private (LogValidity logValidity, string? invalidReason) DeriveLogContextValidity(GetAccommodationRequest request)
    {
        if (request.SearchId == null && request.Page > 0)
        {
            return (LogValidity.Suspect, "Missing searchId on non-first page");
        }

        return (LogValidity.Valid, null);
    }
    
    private async Task<Guid> ResolveSearchId(GetAccommodationRequest request)
    {
        var searchId = request.SearchId;

        if (request.SearchId != null)
        {
            // Assign new SearchId if the searchId have different searchQuery property
            var existingSearchLog = await logRepository.GetSearchLogBySearchIdAsync(request.SearchId.Value);
            if (existingSearchLog != null &&  existingSearchLog.SearchQuery != request.SearchQuery)
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

    private GetAccommodationResponse? CacheLookup(
        string cacheKey,
        GetAccommodationRequest request,
        LogValidity logValidity,
        string? invalidReason,
        Guid searchId)
    {
        if (!cache.TryGetValue<GetAccommodationResponse>(cacheKey, out var cachedResponse)) 
            return null;
        
        var logContext = new LogContext(
            SearchId: searchId,
            Request: request,
            Page: request.Page,
            ResultCount: cachedResponse!.Meta.ResulCount,
            TotalResultCount: cachedResponse.Meta.TotalResult,
            FromCache: true,
            ElapsedMs: 0,
            Validity: logValidity,
            InvalidReason: invalidReason
        );

        EnqueueLog(logContext); // Log search using cache
            
        return cachedResponse;

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

    private void EnqueueLog(LogContext context) =>
        logQueueService.Enqueue(SearchLogEntity.Create(
            context.SearchId,
            DateTime.UtcNow,
            context.Request.SearchQuery,
            context.Request.AccommodationSearchType.ToString(),
            context.Page,
            context.ResultCount,
            context.TotalResultCount,
            context.FromCache,
            context.ElapsedMs,
            null,
            null,
            null,
            context.Validity,
            context.InvalidReason
        ));
    
    #endregion
    
    #region Data Model
    private record LogContext(
        Guid SearchId,
        GetAccommodationRequest Request,
        int Page,
        int ResultCount,
        int TotalResultCount,
        bool FromCache,
        long ElapsedMs,
        LogValidity Validity,
        string? InvalidReason
    );
    #endregion
}