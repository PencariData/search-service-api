using FluentValidation;
using FluentValidation.Results;

namespace SearchService.Application.Extensions;

public static class ValidationExtensions
{
    public static async Task ValidateAndThrowAsync<T>(
        this IValidator<T> validator,
        T instance)
    {
        ValidationResult validationResult = await validator.ValidateAsync(
            instance);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
    }
}