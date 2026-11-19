using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using WineQualityApp.Application.Abstractions;

namespace WineQualityApp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddPythonMlClient(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<PythonServiceOptions>()
            .Bind(configuration.GetSection(PythonServiceOptions.SectionName))
            .Validate(options =>
                Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out var uri) &&
                (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps),
                "PythonService:BaseUrl must be an absolute HTTP or HTTPS URL.")
            .ValidateOnStart();

        services.AddHttpClient<IPythonPredictionClient, PythonMlClient>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<PythonServiceOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl, UriKind.Absolute);
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        return services;
    }
}
