using FluentValidation;
using SearchService.Application.Enums;

namespace SearchService.Application.Dto;

public record GetSuggestionRequest
(
    string Query,
    int Limit,
    Guid? SearchId
);

public class GetSuggestionRequestValidator : AbstractValidator<GetSuggestionRequest>
{
    public GetSuggestionRequestValidator()
    {
        RuleFor(x => x.Query)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.Limit)
            .GreaterThan(0)
            .LessThanOrEqualTo(3);

        RuleFor(x => x.SearchId)
            .Must(id => !id.HasValue || id.Value != Guid.Empty)
            .WithMessage("SearchId must be a valid GUID when provided.");
    }
}