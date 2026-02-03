using AuthenticationService.Configuration;
using AuthenticationService.Controller;


namespace AuthenticationService;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        DotNetEnv.Env.Load();
        var configuration = new Configuration.Configuration();

        SwaggerConfiguration.ConfigurationServices(builder.Services);
        DependencyInjectionConfiguration.ConfigureServices(builder.Services, configuration);

        JwtTokenConfiguration.ConfigureServices(builder.Services);

        var app = builder.Build();

        app.UseCors("AllowOrigin");
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseResponseCompression();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUi();
        }

        app.UseHttpsRedirection();
        app.UseAuthenticationController();

        app.Run();
    }
}



