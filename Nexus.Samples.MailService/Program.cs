using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nexus.Samples.MailClient;
using Nexus.Samples.Sdk;
using Nexus.Samples.Sdk.Models.Shared;

var config = new ConfigurationBuilder()
              .SetBasePath(Environment.CurrentDirectory)
              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
              .AddEnvironmentVariables()
              .Build();

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.Configure<NexusConnectionOptions>(config.GetSection("nexusConnection"));
        services.Configure<AuthMessageSenderOptions>(config.GetSection("mailClientConfiguration"));
        services.AddSingleton<NexusClient>();
        services.AddSingleton<NexusMailClient>();
    })
    .Build();

host.Run();
