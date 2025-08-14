using Microsoft.Extensions.DependencyInjection;
using SearchService.Application.Interfaces.Repositories;
using SearchService.Infrastructure.Repositories;
using SearchService.Infrastructure.ServiceCollectionExtensions;

namespace SearchService.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services)
    {
        services.AddElasticSearch();
        services.AddScoped<IAccommodationRepository, AccommodationRepository>();
    }
}