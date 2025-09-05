using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SearchService.Shared.Models;

namespace SearchService.Shared;

public static class DependencyInjection
{
    public static void AddShared(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ElasticConfiguration>(configuration.GetSection("ElasticConfiguration"));
        services.Configure<CachingOptions>(configuration.GetSection("Caching"));

        // Allow direct ElasticConfiguration injection instead of IOptions<ElasticConfiguration>
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<ElasticConfiguration>>().Value);
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<CachingOptions>>().Value);
    }
}
