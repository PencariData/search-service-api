using FluentValidation;

namespace SearchService.Application.Dto;

public class GetAccommodationSuggestionRequest
{
    public string Query { get; set; } = string.Empty;
    public int Limit { get; set; }
}

public class GetAccommodationSuggestionRequestValidator : AbstractValidator<GetAccommodationSuggestionRequest>
{
    public GetAccommodationSuggestionRequestValidator()
    {
        RuleFor(x => x.Query)
            .NotNull()
            .NotEmpty();
        
        RuleFor(x => x.Limit)
            .GreaterThan(0)
            .LessThanOrEqualTo(4);
    }
}