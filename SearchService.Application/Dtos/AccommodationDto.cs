using SearchService.Domain.Enums;
using SearchService.Domain.ValueObjects;

namespace SearchService.Application.Dtos;

public record AccommodationDto(
    Guid Id,
    string Name,
    string Destination,
    AccommodationType AccommodationType,
    Coordinate Coordinate
);