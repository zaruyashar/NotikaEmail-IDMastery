using Microsoft.AspNetCore.Mvc;
using NotikaEmail_IDMastery.Context;

namespace NotikaEmail_IDMastery.Controllers
{
    public class MessageController : Controller
    {
        private readonly EmailContext _context;

        public MessageController(EmailContext context)
        {
            _context = context;
        }

        public IActionResult Inbox()
        {
            var values = _context.Messages.Where(x => x.ReceiverEmail == "ali@email.com").ToList();
            return View(values);
        }

        public IActionResult Sendbox()
        {
            var values = _context.Messages.Where(x => x.SenderEmail == "ali@email.com").ToList();
            return View(values);
        }

        public IActionResult MessageDetail()
        {
            var value = _context.Messages.Where(x => x.MessageId == 4).FirstOrDefault();
            return View(value);
        }

        [HttpGet]
        public IActionResult ComposeMessage()
        {
            return View();
        }
    }
}
