using SearchService.Domain.Enums;
using SearchService.Domain.ValueObjects;

namespace SearchService.Application.Dto;

public record GetAccommodationResponse(
    Guid Id,
    string Name,
    string Destination,
    AccommodationType AccommodationType,
    Coordinate Coordinate
);