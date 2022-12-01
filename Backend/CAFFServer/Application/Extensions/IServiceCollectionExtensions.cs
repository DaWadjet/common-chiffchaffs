using Application.Services;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection ConfigureApplicationLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(typeof(IServiceCollectionExtensions).Assembly);
        services.ConfigureServices();

        return services;
    }

    private static IServiceCollection ConfigureServices(this IServiceCollection services) 
    {
        services.AddScoped<IFileService,FileService>();
        return services;
    }
}
