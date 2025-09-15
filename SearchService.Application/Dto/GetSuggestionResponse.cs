namespace SearchService.Application.Dto;

public record GetSuggestionResponse(
    SuggestionMetaDto Meta,
    List<string> AccommodationSuggestions, 
    List<string> DestinationSuggestions
);