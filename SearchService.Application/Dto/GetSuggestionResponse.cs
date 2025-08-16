namespace SearchService.Application.Dto;

public record GetSuggestionResponse(
    List<string> AccommodationSuggestions, 
    List<string> DestinationSuggestions);