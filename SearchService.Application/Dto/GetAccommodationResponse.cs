using SearchService.Domain.Enums;
using SearchService.Domain.ValueObjects;

namespace SearchService.Application.Dto;

public record GetAccommodationResponse(
    SearchMetaDto Meta,
    List<AccommodationDto> Accommodations);