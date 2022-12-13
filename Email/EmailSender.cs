using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace Hadddock.Email
{
    public interface IEmailService
    {
        public void SendEmail(string recipientAddress, string subject, string body);
    }

    public class EmailSender : IEmailService
    {
        private const string SENDER_NAME = "DO NOT REPLY";
        private const int MAX_ATTEMPTS = 3;
        private readonly string? smtpClientServer;
        private readonly int smtpClientPort;
        private readonly string? smtpClientLogin;
        private readonly string? smtpClientPassword;
        public EmailSender()
        {
            string appsettingsPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
                + Path.DirectorySeparatorChar + "appsettings.json";

            if (!File.Exists(appsettingsPath))
            {
                throw new FileNotFoundException("appsettings.json not found in execution directory");
            }

            IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", false, true);
            IConfigurationRoot root = builder.Build();
            Console.WriteLine(root["smtpClientServer"]);

            if (root["smtpClientServer"] is null)
                throw new KeyNotFoundException("smtpClientServer key not found in appsettings.json");

            if (root["smtpClientPort"] is null)
                throw new KeyNotFoundException("smtpClientPort key not found in appsettings.json");

            if (root["smtpClientLogin"] is null)
                throw new KeyNotFoundException("smtpClientLogin key not found in appsettings.json");

            this.smtpClientServer = root["smtpClientServer"];
            this.smtpClientLogin = root["smtpClientLogin"];
            this.smtpClientPassword = root["smtpClientPassword"];
            string? smtpClientPortString = root["smtpClientPort"];

            if (smtpClientPortString is not null)
            {
                this.smtpClientPort = int.Parse(smtpClientPortString);
            }

            if (smtpClientLogin is not null)
            {
                try
                {
                    MailAddress from = new(this.smtpClientLogin);
                }
                catch (FormatException)
                {
                    throw new FormatException("Sender email address specified in smtpClientLogin has an invalid format");
                }
            }

        }
        public void SendEmail(string recipientAddress, string subject, string body)
        {
            if (recipientAddress is null)
                throw new ArgumentNullException(nameof(recipientAddress));

            MailAddress to;
            try
            {
                to = new MailAddress(recipientAddress);
            }

            catch (FormatException)
            {
                throw new FormatException("Recipient email address has an invalid format");
            }

            if (this.smtpClientLogin is null)
                throw new NullReferenceException(nameof(this.smtpClientLogin));
            MailAddress from = new(this.smtpClientLogin, SENDER_NAME);
            MailMessage message = new(from, to) { Subject = subject, Body = body };


            SmtpClient client = new(smtpClientServer, smtpClientPort)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(smtpClientLogin, smtpClientPassword),
            };

            SendMessageHelper(client, message);

            static void SendMessageHelper(SmtpClient client, MailMessage message, int attemptNumber = 0)
            {
                try
                {
                    client.Send(message);
                    Console.WriteLine("Message sent successfully");
                    WriteMessageStatus(message, "Message sent");
                }

                catch (SmtpException e)
                {
                    Console.WriteLine("Message not sent:" + e.StatusCode);
                    WriteMessageStatus(message, e.StatusCode.ToString());
                    if (attemptNumber != MAX_ATTEMPTS)
                    {
                        Console.WriteLine("Retrying now");
                        SendMessageHelper(client, message, ++attemptNumber);
                    }

                    else
                        Console.WriteLine(MAX_ATTEMPTS + " retry attempts reached. Aborting.");
                }

                return;

                static void WriteMessageStatus(MailMessage message, string status)
                {
                    using StreamWriter w = File.AppendText("log.csv");
                    {
                        if (message.From is null)
                            message.From = new MailAddress("");
                        w.WriteLine(string.Join(",", new string[]
                        {
                            status,
                            message.From.Address.ToString(),
                            message.To.ToString(),
                            message.Subject,
                            message.Body,
                            DateTime.Today.ToString("dd/MM/yyyy")
                        }));
                    }
                }

            }

        }
    }
}