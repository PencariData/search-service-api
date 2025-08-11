using FluentValidation;
using SearchService.Application.Dtos;
using SearchService.Application.Enums;
using SearchService.Application.Interfaces;
using SearchService.Application.Interfaces.Repositories;
using SearchService.Application.Interfaces.Services;
using SearchService.Domain.Entities;

namespace SearchService.Application.Services;

public class AccommodationService : IAccommodationService
{
    private readonly IAccommodationRepository _accommodationRepository;
    private readonly IValidator<GetAccommodationRequest> _validator;

    public AccommodationService(IAccommodationRepository accommodationRepository, IValidator<GetAccommodationRequest> validator)
    {
        _accommodationRepository = accommodationRepository;
        _validator = validator;
    }

    public async Task<List<AccommodationDto>> SearchAccommodationAsync(GetAccommodationRequest request)
    {
        // Validate input here
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        IEnumerable<AccommodationEntity> accommodations;

        switch (request.AccommodationSearchType)
        {
            case AccommodationSearchType.Name:
                accommodations = await _accommodationRepository.GetByNameAsync(
                    request.Name!, // validated not null
                    request.Limit);
                break;

            case AccommodationSearchType.Destination:
                accommodations = await _accommodationRepository.GetByDestinationAsync(
                    request.Destination!, // validated not null
                    request.Limit);
                break;

            default:
                throw new InvalidOperationException("SearchType undefined");
        }

        return accommodations.Select(x => new AccommodationDto(
            x.Id,
            x.Name,
            x.FullDestination,
            x.AccommodationType,
            x.Coordinate)).ToList();
    }
}