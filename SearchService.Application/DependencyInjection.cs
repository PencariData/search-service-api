using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SearchService.Application.Interfaces.Services;
using SearchService.Application.Services;

namespace SearchService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAccommodationService, AccommodationService>();
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        return services;
    }
}