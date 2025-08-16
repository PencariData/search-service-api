using FluentValidation;
using SearchService.Application.Dto;
using SearchService.Application.Enums;
using SearchService.Application.Interfaces.Repositories;
using SearchService.Application.Interfaces.Services;

namespace SearchService.Application.Services;

public class AccommodationService(
    IAccommodationRepository accommodationRepository,
    IValidator<GetAccommodationRequest> validator)
    : IAccommodationService
{
    public async Task<GetAccommodationResponse> SearchAccommodationsAsync(GetAccommodationRequest request)
    {
        var validationResult = await validator.ValidateAsync(request);
        
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var accommodations = request.AccommodationSearchType switch
        {
            AccommodationSearchType.ByName => await accommodationRepository.GetByNameAsync(
                request.SearchQuery,
                request.Limit),
            
            AccommodationSearchType.ByDestination => await accommodationRepository.GetByDestinationAsync(
                request.SearchQuery,
                request.Limit),
            
            _ => throw new InvalidOperationException("SearchType undefined")
        };

        return new GetAccommodationResponse(accommodations.Select(accommodation => new AccommodationDto(
                accommodation.Id,
                accommodation.Name,
                accommodation.FullDestination,
                accommodation.AccommodationType,
                accommodation.Coordinate)
            ).ToList()
        );
    }
}