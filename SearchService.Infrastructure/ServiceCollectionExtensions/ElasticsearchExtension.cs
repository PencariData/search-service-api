using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.DependencyInjection;
using SearchService.Shared.Models;

namespace SearchService.Infrastructure.ServiceCollectionExtensions;

public static class ElasticsearchExtension
{
    public static void AddElasticSearch(this IServiceCollection services)
    {
        services.AddSingleton(sp =>
        {
            var config = sp.GetRequiredService<ElasticConfiguration>();

            var settings = new ElasticsearchClientSettings(new Uri(config.ElasticUrl));
            return new ElasticsearchClient(settings);
        });
    }
}