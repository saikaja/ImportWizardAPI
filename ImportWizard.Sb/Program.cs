using ImportWizard.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ImportWizard.Sb.HostedServices; // ProfileProcessorService

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(cfg =>
    {
        cfg.AddJsonFile("appsettings.json", optional: true)
           .AddEnvironmentVariables();
    })
    .ConfigureLogging(lb =>
    {
        lb.ClearProviders();
        lb.AddConsole(); // shows up in WebJob logs
    })
    .ConfigureServices((ctx, svcs) =>
    {
        // DB: read from Connection strings (“ConnectionStrings:DefaultConnection”)
        var conn = ctx.Configuration.GetConnectionString("DefaultConnection")
                   ?? ctx.Configuration["ConnectionStrings:DefaultConnection"];
        if (string.IsNullOrWhiteSpace(conn))
            throw new InvalidOperationException("Missing ConnectionStrings:DefaultConnection");

        svcs.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(conn));

        // Your ServiceBus settings are consumed inside ProfileProcessorService via IOptions<ServiceBusSettings>.
        svcs.Configure<ImportWizard.Sb.Settings.ServiceBusSettings>(
            ctx.Configuration.GetSection("AzureServiceBus"));

        // Provide the ServiceBusClient for the processor ctor
        svcs.AddSingleton(sp =>
        {
            var s = sp.GetRequiredService<
                Microsoft.Extensions.Options.IOptions<ImportWizard.Sb.Settings.ServiceBusSettings>>().Value;
            return new Azure.Messaging.ServiceBus.ServiceBusClient(s.ConnectionString);
        });

        // Register the worker
        svcs.AddHostedService<ProfileProcessorService>();
    })
    .Build();

await host.RunAsync();
