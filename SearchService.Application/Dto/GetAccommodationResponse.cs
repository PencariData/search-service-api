using SearchService.Domain.Enums;
using SearchService.Domain.ValueObjects;

namespace SearchService.Application.Dto;

public record GetAccommodationResponse(List<AccommodationDto> Accommodations);