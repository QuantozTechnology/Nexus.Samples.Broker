using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nexus.Samples.MailClient;
using Nexus.Samples.MailService;
using Nexus.Samples.Sdk;
using System;

[assembly: WebJobsStartup(typeof(Startup))]
namespace Nexus.Samples.MailService
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = new ConfigurationBuilder()
              .SetBasePath(Environment.CurrentDirectory)
              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
              .AddEnvironmentVariables()
              .Build();

            builder.Services.Configure<AuthMessageSenderOptions>(config.GetSection("mailClientConfiguration"));
            builder.Services.AddSingleton(new NexusClient(config));
            builder.Services.AddSingleton<NexusMailClient>();
        }
    }
}