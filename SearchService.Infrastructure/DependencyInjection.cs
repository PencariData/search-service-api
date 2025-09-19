using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SearchService.Application.Interfaces.Repositories;
using SearchService.Infrastructure.Helpers.HttpRequestHandler;
using SearchService.Infrastructure.Helpers.Seeding;
using SearchService.Infrastructure.Repositories;
using SearchService.Infrastructure.ServiceCollectionExtensions;

namespace SearchService.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient();
        services.AddElasticSearch();
        services.AddBackgroundServices();
        services.AddPostgreSql(configuration);

        services.AddScoped<IElasticsearchRequestHelper, ElasticsearchRequestHelper>();
        services.AddScoped<IElasticSeeder, ElasticSeeder>();
        
        services.AddScoped<IAccommodationRepository, AccommodationRepository>();
        services.AddScoped<IDestinationRepository, DestinationRepository>();
        services.AddScoped<ISearchLogRepository, SearchLogRepository>();
        services.AddScoped<ISuggestionLogRepository, SuggestionLogRepository>();
    }
}