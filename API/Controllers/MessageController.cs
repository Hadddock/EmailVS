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
        private IEmailService _emailService;

        public MessageController(ILogger<MessageController> logger, IEmailService emailService)
        {
            _logger = logger;
            _emailService = emailService;
        }

        [HttpPost(Name = "SendEmail")]
        public void Post(EmailDTO messageDto)
        {
            if
            (
                messageDto.messageBody is not null &&
                messageDto.messageSubject is not null &&
                messageDto.recipientEmail is not null
            )

            {
                _emailService.SendEmail(messageDto.recipientEmail, messageDto.messageSubject, messageDto.messageBody);
            }

        }
    }
}