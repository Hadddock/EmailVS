using Hadddock.Email;

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

            IEmailService e = new EmailSender();
            e.SendEmail(args[0], args[1], args[2]);
        }
    }
}