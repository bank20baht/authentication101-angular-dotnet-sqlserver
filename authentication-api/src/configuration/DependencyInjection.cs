using System.IO.Compression;
using Microsoft.AspNetCore.ResponseCompression;
using Shared.Utils;

namespace AuthenticationService.Configuration;

public static class DependencyInjectionConfiguration
{
    public static void ConfigureServices(IServiceCollection services, Configuration configuration)
    {
        services.AddHttpContextAccessor();

        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
        });

        services.Configure<BrotliCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Fastest;
        });

        services.Configure<GzipCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.SmallestSize;
        });

        var allowedOrigins = Environment.GetEnvironmentVariable("CORS_ORIGINS")?
            .Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);


        services.AddCors(options =>
        {
            options.AddPolicy("AllowOrigin", policy =>
            {

                policy.WithOrigins(allowedOrigins ?? Array.Empty<string>())
                                   .AllowAnyMethod()
                                   .AllowAnyHeader()
                                   .AllowCredentials();
            });
        });

        services.AddSingleton(configuration);

        services.AddTransient<AccessTokenService>();
        services.AddTransient<RefreshTokenService>();
    }

}