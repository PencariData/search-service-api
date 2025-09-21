using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SearchService.Application.Interfaces.Services;
using SearchService.Application.Services;

namespace SearchService.Application;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAccommodationSearchService, AccommodationSearchService>();
        services.AddScoped<IAccommodationInteractionService, AccommodationInteractionService>();
        services.AddScoped<ISuggestionSearchService, SuggestionSearchService>();
        services.AddScoped<ISuggestionInteractionService, SuggestionInteractionService>();
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    }
}