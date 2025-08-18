using FluentValidation;
using SearchService.Application.Dto;
using SearchService.Application.Interfaces.Repositories;
using SearchService.Application.Interfaces.Services;
using ValidationException = FluentValidation.ValidationException;

namespace SearchService.Application.Services;

public class SearchSuggestionService(
    IAccommodationRepository accommodationRepository,
    IDestinationRepository destinationRepository,
    IValidator<GetSuggestionRequest> getSuggestionValidator) : ISearchSuggestionService
{
    public async Task<GetSuggestionResponse> GetSuggestionsAsync(GetSuggestionRequest request)
    {
        var validationResult = await getSuggestionValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
        
        // Get accommodation suggestion
        var accommodationSuggestions =
            await accommodationRepository.GetSuggestionsAsync(request.Query, request.Limit);
        
        // Get destination suggestion 
        var destinationSuggestions =
            await destinationRepository.GetDestinationSuggestionsAsync(request.Query, request.Limit);
        
        // Return 
        return new GetSuggestionResponse(
            accommodationSuggestions.ToList(), 
            destinationSuggestions.ToList());
    }
}