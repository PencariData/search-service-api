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
        // Validation data
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var logValidity = LogValidity.Valid;
        string? invalidReason = null;
        
        if (request.SearchId == null && request.Page > 0)
        {
            logValidity = LogValidity.Suspect;
            invalidReason = "Missing searchId on non-first page";
        }
        
        var searchId = request.SearchId;

        if (request.SearchId != null)
        {
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
        
        // Cache lookups
        var cacheKey = $"search:{request.SearchQuery}:{request.Page}:{request.AccommodationSearchType}";
        if (cache.TryGetValue<GetAccommodationResponse>(cacheKey, out var cachedResponse))
        {
            EnqueueLog(searchId!.Value, request, request.Page,cachedResponse!.Meta.ResulCount, cachedResponse.Meta.TotalResult,true, 0, logValidity, invalidReason);
            return cachedResponse;
        }
        
        // Search execution
        var timer = Stopwatch.StartNew();
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
        timer.Stop();
        
        // Mapping
        var dtoList = searchResult.Results.Select(MapToDto).ToList();

        // Meta & Response
        var response = new GetAccommodationResponse(
            Meta: new SearchMetaDto(
                SearchId: searchId!.Value,
                Page: request.Page,
                ResulCount: dtoList.Count,
                TotalResult: searchResult.Total
            ),
            Accommodations: dtoList
        );
        
        // Logging
        EnqueueLog(searchId.Value, request, request.Page, dtoList.Count, dtoList.Count, false, timer.ElapsedMilliseconds, logValidity, invalidReason);
        
        // Cache Store
        cache.SetWithConfig(cacheKey, response, cachingOptions.ResultCacheDurationMinutes);

        return response;
    }
    
    private static AccommodationDto MapToDto(AccommodationEntity entity) =>
        new(entity.Id, entity.Name, entity.FullDestination, entity.AccommodationType, entity.Coordinate);

    private void EnqueueLog(Guid searchId, GetAccommodationRequest request,int page, int resultCount, int totalResultCount, bool fromCache, long elapsedMs, LogValidity validity, string? invalidReason) =>
        logQueueService.Enqueue(SearchLogEntity.Create(
            searchId,
            DateTime.UtcNow,
            request.SearchQuery,
            request.AccommodationSearchType.ToString(),
            page,
            resultCount,
            totalResultCount,
            fromCache,
            elapsedMs,
            null,
            null,
            null,
            validity,
            invalidReason
        ));
}