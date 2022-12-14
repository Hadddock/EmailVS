using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
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
        public EmailSender(string smtpClientServer, string smtpClientPort, string smtpClientLogin, string smtpClientPassword)
        {
            if (smtpClientServer is null)
                throw new ArgumentNullException(nameof(smtpClientServer));
            if (smtpClientPort is null)
                throw new ArgumentNullException(nameof(smtpClientPort));
            if (smtpClientLogin is null)
                throw new ArgumentNullException(nameof(smtpClientLogin));
            if (smtpClientPassword is null)
                throw new ArgumentNullException(nameof(smtpClientPassword));

            this.smtpClientServer = smtpClientServer;
            this.smtpClientPort = int.Parse(smtpClientPort);
            this.smtpClientLogin = smtpClientLogin;
            this.smtpClientPassword = smtpClientPassword;

            try
            {
                MailAddress from = new(this.smtpClientLogin);
            }

            catch (FormatException)
            {
                throw new FormatException("Sender email address specified in smtpClientLogin has an invalid format");
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


            SmtpClient client = new(this.smtpClientServer, this.smtpClientPort)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(this.smtpClientLogin, this.smtpClientPassword),
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