using Microsoft.Extensions.DependencyInjection;
using WeatherConsoleApp;
using Hadddock.Email;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureHostConfiguration(configHost =>
    {
        configHost.SetBasePath(Directory.GetCurrentDirectory());
        configHost.AddJsonFile("appsettings.json", optional: false);
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<WeatherUpdater>()
            .AddSingleton<IEmailService>(emailService => new EmailSender
            (
                hostContext.Configuration.GetSection("smtpClientServer").Value,
                hostContext.Configuration.GetSection("smtpClientPort").Value,
                hostContext.Configuration.GetSection("smtpClientLogin").Value,
                hostContext.Configuration.GetSection("smtpClientPassword").Value
            ));
    })
    .Build();
host.Run();
