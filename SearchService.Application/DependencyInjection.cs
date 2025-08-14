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
        services.AddScoped<IAccommodationService, AccommodationService>();
        services.AddScoped<ISearchSuggestionService, SearchSuggestionService>();
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    }
}