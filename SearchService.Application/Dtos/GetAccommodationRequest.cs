using FluentValidation;
using SearchService.Application.Enums;

namespace SearchService.Application.Dtos;

public class GetAccommodationRequest
{
    public string? Name { get; set; }
    public string? Destination  { get; set; }
    public AccommodationSearchType AccommodationSearchType { get; set; }
    public int Limit { get; set; }
}

public class GetAccommodationRequestValidator : AbstractValidator<GetAccommodationRequest>
{
    public GetAccommodationRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .When(x => x.AccommodationSearchType == AccommodationSearchType.Name);

        RuleFor(x => x.Destination)
            .NotEmpty()
            .When(x => x.AccommodationSearchType == AccommodationSearchType.Destination);

        RuleFor(x => x.Limit)
            .GreaterThan(0)
            .LessThanOrEqualTo(30);

        RuleFor(x => x.AccommodationSearchType)
            .IsInEnum()
            .WithMessage("SearchType must be a valid value.");
    }
}