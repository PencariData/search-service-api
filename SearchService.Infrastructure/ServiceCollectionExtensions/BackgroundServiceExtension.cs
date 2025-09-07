using System.Threading.Channels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SearchService.Application.Interfaces.Services;
using SearchService.Domain.Entities;
using SearchService.Infrastructure.Services;

namespace SearchService.Infrastructure.ServiceCollectionExtensions;

public static class BackgroundServiceExtension
{
    public static void AddBackgroundServices(this IServiceCollection services)
    {
        services.AddSingleton(Channel.CreateUnbounded<SearchLogEntity>());
        services.AddHostedService<SearchLogBackgroundService>();

        services.AddSingleton<ILogQueueService, LogQueueService>();
    }
}