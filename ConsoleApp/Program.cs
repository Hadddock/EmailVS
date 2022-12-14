using Hadddock.Email;
using Microsoft.Extensions.Configuration;

namespace Dummy
{
    public class ConsoleApp
    {
        static void Main(string[] args)
        {
            if (args.Length != 3)
                throw new ArgumentNullException(nameof(args),
                    "Recipient address, message subject, " +
                    "and message body Required as arguments");
            IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", false);
            IConfigurationRoot root = builder.Build();

            IEmailService e = new EmailSender
            (
                root["smtpClientServer"], 
                root["smtpClientPort"], 
                root["smtpClientLogin"], 
                root["smtpClientPassword"]
            );
            e.SendEmail(args[0], args[1], args[2]);
        }
    }
}