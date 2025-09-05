using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using SearchService.Application.Dto;
using SearchService.Application.Enums;
using SearchService.Application.Interfaces.Repositories;
using SearchService.Application.Interfaces.Services;
using SearchService.Shared.Extensions;
using SearchService.Shared.Models;

namespace SearchService.Application.Services;

public class AccommodationService(
    IAccommodationRepository accommodationRepository,
    IValidator<GetAccommodationRequest> validator,
    IMemoryCache cache,
    CachingOptions cachingOptions)
    : IAccommodationService
{
    public async Task<GetAccommodationResponse> SearchAccommodationsAsync(GetAccommodationRequest request)
    {
        var validationResult = await validator.ValidateAsync(request);
        
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var cacheKey = $"search:{request.SearchQuery}{request.AccommodationSearchType}";
        
        if (cache.TryGetValue<GetAccommodationResponse>(cacheKey, out var searchResult))
        {
            if (searchResult != null) 
                return searchResult;
        }

        var accommodations = request.AccommodationSearchType switch
        {
            AccommodationSearchType.FreeSearch => await accommodationRepository.GetByMultipleFieldAsync(
                ["destinationName.ngram", "name.ngram"],
                request.SearchQuery,
                request.Limit),
            
            AccommodationSearchType.ByDestination => await accommodationRepository.GetByFieldAsync(
                "destinationName",
                request.SearchQuery,
                request.Limit),
            
            AccommodationSearchType.ByName => await accommodationRepository.GetByFieldAsync(
                "name",
                request.SearchQuery,
                request.Limit),
            
            _ => throw new InvalidOperationException("SearchType undefined")
        };

        var response = new GetAccommodationResponse(accommodations.Select(accommodation => new AccommodationDto(
                accommodation.Id,
                accommodation.Name,
                accommodation.FullDestination,
                accommodation.AccommodationType,
                accommodation.Coordinate)
            ).ToList()
        );
        
        cache.SetWithConfig(cacheKey, response, cachingOptions.SuggestionCacheDurationMinutes);

        return response;
    }
}