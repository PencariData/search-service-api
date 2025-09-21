using System.Threading.Channels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SearchService.Application.Interfaces.Repositories;
using SearchService.Application.Interfaces.Services;
using SearchService.Domain.Entities;
using SearchService.Domain.Events;
using SearchService.Infrastructure.Repositories;
using SearchService.Infrastructure.Services;

namespace SearchService.Infrastructure.ServiceCollectionExtensions;

public static class BackgroundServiceExtension
{
    public static void AddBackgroundServices(this IServiceCollection services)
    {
        services.AddSingleton(Channel.CreateUnbounded<SearchEvent>());
        
        services.AddHostedService<BackgroundLogService<SearchEvent, ISearchEventRepository>>();
        
        services.AddSingleton(typeof(ILogQueueService<>), typeof(LogQueueService<>));
    }
}