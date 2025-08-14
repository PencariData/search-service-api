using System.ComponentModel.DataAnnotations;
using FluentValidation;
using SearchService.Application.Dto;
using SearchService.Application.Interfaces.Repositories;
using SearchService.Application.Interfaces.Services;
using ValidationException = FluentValidation.ValidationException;

namespace SearchService.Application.Services;

public class SearchSuggestionService(
    IAccommodationRepository accommodationRepository,
    IValidator<GetAccommodationSuggestionRequest> validator) : ISearchSuggestionService
{
    public async Task<List<string>> SearchAccommodationSuggestionsAsync(GetAccommodationSuggestionRequest request)
    {
        var validationResult = await validator.ValidateAsync(request);
        
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
        
        var suggestions = await accommodationRepository.GetHotelSuggestionsAsync(request.Query, request.Limit);
        
        return suggestions.ToList();
    }
}