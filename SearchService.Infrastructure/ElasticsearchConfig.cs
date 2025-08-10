using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SearchService.Infrastructure;

public static class ElasticsearchConfig
{
    public static IServiceCollection AddElasticSearch(this IServiceCollection services, IConfiguration configuration)
    {
        var uri = configuration["ElasticConfiguration:ElasticUrl"] 
                  ?? throw new InvalidOperationException("ElasticUrl configuration is not defined");
        
        var settings = new ElasticsearchClientSettings(new Uri(uri));

        var client = new ElasticsearchClient(settings);

        services.AddSingleton(client);

        return services;
    }

}