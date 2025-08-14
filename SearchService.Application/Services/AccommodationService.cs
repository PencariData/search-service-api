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
    public async Task<List<GetAccommodationResponse>> SearchAccommodationsAsync(GetAccommodationRequest request)
    {
        var validationResult = await validator.ValidateAsync(request);
        
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var accommodations = request.AccommodationSearchType switch
        {
            AccommodationSearchType.Name => await accommodationRepository.GetByNameAsync(
                request.Name!, // validated not null
                request.Limit),
            
            AccommodationSearchType.Destination => await accommodationRepository.GetByDestinationAsync(
                request.Destination!, // validated not null
                request.Limit),
            
            _ => throw new InvalidOperationException("SearchType undefined")
        };

        if (accommodations == null)
        {
            return [];
        }

        return accommodations.Select(x => new GetAccommodationResponse(
            x.Id,
            x.Name,
            x.FullDestination,
            x.AccommodationType,
            x.Coordinate)).ToList();
    }
}