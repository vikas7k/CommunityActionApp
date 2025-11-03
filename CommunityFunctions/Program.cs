using CommunityFunctions.Data;
using CommunityFunctions.Security;
using CommunityFunctions.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration((context, b) =>
       {
           b.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);         
           b.AddEnvironmentVariables();
       })
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;
        var secretKey = configuration["Jwt:SigningKey"] ?? throw new InvalidOperationException("Jwt:Secret configuration is missing.");
        var issuer = configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer configuration is missing.");
        var audience = configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience configuration is missing.");
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        var config = context.Configuration;
        var conn = config.GetConnectionString("SqlServer") ?? config["ConnectionStrings:SqlServer"];
        services.AddDbContext<AppDbContext>(opts => opts.UseSqlServer(conn));
        services.AddSingleton(new JwtService(secretKey, issuer, audience));
        services.AddSingleton<IJwtValidator, JwtValidator>();
    })
    .Build();

host.Run();