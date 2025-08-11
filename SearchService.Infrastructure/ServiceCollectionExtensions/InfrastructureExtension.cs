using Microsoft.Extensions.DependencyInjection;
using SearchService.Application.Interfaces;
using SearchService.Application.Interfaces.Repositories;
using SearchService.Infrastructure.Repositories;

namespace SearchService.Infrastructure.ServiceCollectionExtensions;

public static class InfrastructureExtension
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IAccommodationRepository, AccommodationRepository>();
        
        return services;
    }
}