using SearchService.Domain.Enums;
using SearchService.Domain.ValueObjects;

namespace SearchService.Application.Dto;

public record AccommodationDto (
    Guid Id, 
    string Name, 
    string Destination, 
    AccommodationType AccommodationType, 
    Coordinate Coordinate);