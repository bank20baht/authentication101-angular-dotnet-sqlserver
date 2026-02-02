using System.Diagnostics.CodeAnalysis;
using Microsoft.OpenApi;

namespace AuthenticationService.Configuration;

[ExcludeFromCodeCoverage]
public static class SwaggerConfiguration
{
    public static void ConfigurationServices(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddOpenApiDocument(option =>
        {
            option.Title = "authentication-api";
            option.Version = "v1";
        });
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "product-management-api", Version = "v1" });
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
        });
    }
}