using Cortex.Mediator.Commands;
using Cortex.Mediator.DependencyInjection;
using Cortex.Mediator.Queries;
using HngStageOne.Infrastructure.Services;
using HngStageOne.Application.Abstractions.Services;
using HngStageOne.Configurations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using HngStageOne.Infrastructure.Persistence.Context;

namespace HngStageOne.Extensions;


/// <summary>
/// Provides extension methods for registering API-layer services.
/// </summary>
/// <remarks>
/// This class centralizes API-related service registrations,
/// keeping the application's start up configuration clean,
/// modular, and maintainable.
/// </remarks>
public static class ApiServicesRegistration
{
    /// <summary>
    /// Registers API services including controllers and OpenAPI documentation.
    /// </summary>
    /// <param name="services">
    /// The service collection used to register application services.
    /// </param>
    /// <param name="configuration">
    /// The application configuration instance.
    /// </param>
    /// <returns>
    /// The updated <see cref="IServiceCollection"/> for chaining.
    /// </returns>
    /// <remarks>
    /// This method configures services such as:
    /// <list type="bullet">
    /// <item><description>Controller support for REST endpoints</description></item>
    /// <item><description>OpenAPI/Swagger documentation</description></item>
    /// </list>
    ///
    /// Additional API-level services such as filters, versioning,
    /// and middleware dependencies may be registered here.
    /// </remarks>
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.SuppressModelStateInvalidFilter = true;
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var firstError = context.ModelState.Values
                            .SelectMany(v => v.Errors)
                            .FirstOrDefault()?.ErrorMessage;

                        var response = new
                        {
                            status = "error",
                            message = firstError ?? "Invalid request"
                        };

                        return new BadRequestObjectResult(response);
                    };
                });

        services.AddOpenApi();

        AddCortexMediator(services);
        AddDbContext(services, configuration);

        // Add CORS policy
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
        AddHttpClients(services, configuration);

        return services;
    }

    private static void AddCortexMediator(IServiceCollection services)
    {
        services.AddCortexMediator(
            handlerAssemblyMarkerTypes: [typeof(ApiServicesRegistration)],
            configure: options =>
            {
                //Register logging behavior
                options.AddDefaultBehaviorsWithExceptionHandling();

            });

        var assembly = Assembly.GetExecutingAssembly();
        services.Scan(scan => scan.FromAssemblies(assembly)
            .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime()
        );
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <exception cref="ArgumentNullException"></exception>
    private static void AddDbContext(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("ProfileDbConnection") ??
                               throw new ArgumentNullException(nameof(configuration));
        services.AddDbContext<ProfileDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });
    }

    /// <summary>
    /// Registers HTTP clients and related handlers for external identity verification services.
    /// </summary>
    /// <remarks>
    /// Configures the identity provider HTTP client.
    /// </remarks>
    private static void AddHttpClients(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();

        services.Configure<GenderizeSettings>(configuration.GetSection(nameof(GenderizeSettings)));
        services.AddHttpClient<IGenderizeClient, GenderizeClient>((sp, httpClient) =>
        {
            var settings = sp.GetRequiredService<IOptionsMonitor<GenderizeSettings>>().CurrentValue;

            if (string.IsNullOrWhiteSpace(settings.BaseUrl))
            {
                throw new InvalidOperationException($"Genderize API {nameof(GenderizeSettings.BaseUrl)} configuration is missing from app settings.");
            }

            httpClient.BaseAddress = new Uri(settings.BaseUrl);
        });

        services.Configure<AgifySettings>(configuration.GetSection(nameof(AgifySettings)));
        services.AddHttpClient<IAgifyClient, AgifyClient>((sp, httpClient) =>
        {
            var settings = sp.GetRequiredService<IOptionsMonitor<AgifySettings>>().CurrentValue;
            if (string.IsNullOrWhiteSpace(settings.BaseUrl))
            {
                throw new InvalidOperationException($"Agify API {nameof(AgifySettings.BaseUrl)} configuration is missing from app settings.");
            }

            httpClient.BaseAddress = new Uri(settings.BaseUrl);
        });

        services.Configure<NationalizeSettings>(configuration.GetSection(nameof(NationalizeSettings)));
        services.AddHttpClient<INationalizeClient, NationalizeClient>((sp, httpClient) =>
        {
            var settings = sp.GetRequiredService<IOptionsMonitor<NationalizeSettings>>().CurrentValue;
            if (string.IsNullOrWhiteSpace(settings.BaseUrl))
            {
                throw new InvalidOperationException($"Nationalize API {nameof(NationalizeSettings.BaseUrl)} configuration is missing from app settings.");
            }

            httpClient.BaseAddress = new Uri(settings.BaseUrl);
        });
    }
}
