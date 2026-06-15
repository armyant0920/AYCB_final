using CorporateSite.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CorporateSite.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<NewsService>();
        return services;
    }
}
