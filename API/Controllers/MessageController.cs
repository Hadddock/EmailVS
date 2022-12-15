using Microsoft.AspNetCore.Mvc;
using API.DTO;
using Hadddock.Email;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessageController : ControllerBase
    {
       
        private readonly ILogger<MessageController> _logger;
        private readonly IEmailService _emailService;

        public MessageController(ILogger<MessageController> logger, IEmailService emailService)
        {
            _logger = logger;
            _emailService = emailService;
        }

        [HttpPost(Name = "SendEmail")]
        public void Post(EmailDTO emailDto)
        {
            if
            (
                emailDto.messageBody is not null &&
                emailDto.messageSubject is not null &&
                emailDto.recipientEmail is not null
            )

            {
                try
                {
                    _emailService.SendEmail(emailDto.recipientEmail, emailDto.messageSubject, emailDto.messageBody);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}