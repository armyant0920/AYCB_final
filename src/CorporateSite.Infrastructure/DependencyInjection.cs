using CorporateSite.Application.Abstractions.Content;
using CorporateSite.Application.Abstractions.Repositories;
using CorporateSite.Application.Abstractions.Security;
using CorporateSite.Application.Abstractions.Storage;
using CorporateSite.Infrastructure.Content;
using CorporateSite.Infrastructure.Data;
using CorporateSite.Infrastructure.Data.Repositories;
using CorporateSite.Infrastructure.Security;
using CorporateSite.Infrastructure.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace CorporateSite.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<DataProviderFactory>();

        services.AddScoped<INewsRepository, NewsRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddSingleton<IPasswordHashService, PasswordHashService>();
        services.AddScoped<IUserAuthenticator, LocalUserAuthenticator>();

        services.AddSingleton<IHtmlSanitizerService, HtmlSanitizerService>();
        services.AddScoped<IFileStorageService, LocalFileStorageService>();

        return services;
    }
}
