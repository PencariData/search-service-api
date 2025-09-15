using FluentValidation;
using SearchService.Application.Enums;

namespace SearchService.Application.Dto;

public record GetAccommodationRequest(
    string SearchQuery,
    AccommodationSearchType AccommodationSearchType,
    int Limit,
    int Page,
    Guid? SearchId
);

public class GetAccommodationRequestValidator : AbstractValidator<GetAccommodationRequest>
{
    public GetAccommodationRequestValidator()
    {
        RuleFor(x => x.SearchId)
            .Must(id => !id.HasValue || id.Value != Guid.Empty)
            .WithMessage("SearchId must be a valid GUID when provided.");
        
        RuleFor(x => x.SearchQuery)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.Limit)
            .GreaterThan(0)
            .LessThanOrEqualTo(30);

        RuleFor(x => x.AccommodationSearchType)
            .IsInEnum()
            .WithMessage("SearchType must be a valid value.");

        RuleFor(x => x.Page)
            .GreaterThan(-1);
    }
}