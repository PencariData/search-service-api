using FluentValidation;
using SearchService.Application.Enums;

namespace SearchService.Application.Dto;

public record GetAccommodationRequest(
    string SearchQuery,
    AccommodationSearchType AccommodationSearchType,
    int Limit
);

public class GetAccommodationRequestValidator : AbstractValidator<GetAccommodationRequest>
{
    public GetAccommodationRequestValidator()
    {
        RuleFor(x => x.SearchQuery)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.Limit)
            .GreaterThan(0)
            .LessThanOrEqualTo(30);

        RuleFor(x => x.AccommodationSearchType)
            .IsInEnum()
            .WithMessage("SearchType must be a valid value.");
    }
}