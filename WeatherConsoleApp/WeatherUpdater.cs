using Hadddock.Email;
using Microsoft.Extensions.Hosting;

namespace WeatherConsoleApp
{
    class WeatherUpdater : IHostedService
    {
        private IEmailService _emailService;
        private readonly IHostApplicationLifetime _appLifetime;
        private string ? currentWeatherConditions;
        private readonly List<string> mailingList = new() {"example@test.com"};

        private static readonly string[] WeatherSummaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" 
        };
        
        public WeatherUpdater(IHostApplicationLifetime hostApplicationLifetime, IEmailService emailService)
        {
            _appLifetime = hostApplicationLifetime;
            _emailService = emailService;
           
        }

        public void UpdateWeather()
        {
            Random r = new();
            currentWeatherConditions = WeatherSummaries[r.Next(0, WeatherSummaries.Length)];
            SendWeatherUpdate();
        }
        public void SendWeatherUpdate()
        {
            foreach (string address in mailingList)
            {
                _emailService.SendEmail(address, "Weather Update", "The weather is " + currentWeatherConditions);
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            UpdateWeather();
            _appLifetime.StopApplication();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
