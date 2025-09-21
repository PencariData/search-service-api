using System.Diagnostics;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SearchService.Application.Dto;
using SearchService.Application.Interfaces.Repositories;
using SearchService.Application.Interfaces.Services;
using SearchService.Domain.Events;
using SearchService.Shared.Extensions;
using SearchService.Shared.Models;

namespace SearchService.Application.Services;

public class SuggestionSearchService(
    IAccommodationRepository accommodationRepository,
    IDestinationRepository destinationRepository,
    IValidator<GetSuggestionRequest> validator,
    IMemoryCache cache,
    CachingOptions cachingOptions,
    ILogQueueService<SearchEvent> logQueueService,
    ILogger<SuggestionSearchService> logger) : ISuggestionSearchService
{
    public async Task<GetSuggestionResponse> GetSuggestionsAsync(GetSuggestionRequest request)
    {
        // Validate request
        await validator.ValidateAndThrowAsync(request);

        var sessionId = request.SessionId ?? Guid.Empty;
        if (sessionId == Guid.Empty)
            logger.LogWarning("{TimeStamp} - Request with empty sessionId", DateTime.UtcNow);

        // Cache key (include limit for correctness)
        var cacheKey = $"suggestion:{request.Query}:{request.Limit}";
        var cacheData = CacheLookup(cacheKey);
        if (cacheData != null)
        {
            // Log cache hit
            EnqueueLog(new SuggestionsShown(
                sessionId: sessionId,
                searchId: Guid.NewGuid(),
                suggestions: cacheData.AccommodationSuggestions
                    .Concat(cacheData.DestinationSuggestions)
            ));

            return cacheData;
        }

        // Fetch suggestions
        var stopwatch = Stopwatch.StartNew();
        var accommodationSuggestions = (await accommodationRepository.GetSuggestionsAsync(request.Query, request.Limit)).ToList();
        var destinationSuggestions = (await destinationRepository.GetDestinationSuggestionsAsync(request.Query, request.Limit)).ToList();
        stopwatch.Stop();

        var response = new GetSuggestionResponse(
            Meta: new SuggestionMetaDto(
                SessionId: sessionId,
                accommodationSuggestionCount: accommodationSuggestions.Count,
                destinationSuggestionCount: destinationSuggestions.Count
            ),
            AccommodationSuggestions: accommodationSuggestions,
            DestinationSuggestions: destinationSuggestions
        );

        // Log fresh suggestions
        EnqueueLog(new SuggestionsShown(
            sessionId: sessionId,
            searchId: Guid.NewGuid(),
            suggestions: accommodationSuggestions.Concat(destinationSuggestions)
        ));

        // Cache result
        cache.SetWithConfig(cacheKey, response, cachingOptions.SuggestionCacheDurationMinutes);

        return response;
    }

    #region Private Methods

    private GetSuggestionResponse? CacheLookup(string cacheKey)
    {
        return !cache.TryGetValue<GetSuggestionResponse>(cacheKey, out var cachedResponse) ? null : cachedResponse;
    }

    private void EnqueueLog(SearchEvent evt) =>
        logQueueService.Enqueue(evt);

    #endregion
}
