using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NotikaEmail_IDMastery.Context;
using NotikaEmail_IDMastery.Entities;

namespace NotikaEmail_IDMastery.Controllers
{
    public class MessageController : Controller
    {
        private readonly EmailContext _context;
        private readonly UserManager<AppUser> _userManager;

        public MessageController(EmailContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Inbox()
        {
            CreatedAtRoute user = await _userManager.FindByNameAsync(User.Identity.Name)

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
