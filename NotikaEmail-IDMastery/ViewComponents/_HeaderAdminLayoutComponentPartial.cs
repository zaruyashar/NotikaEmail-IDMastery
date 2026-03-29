using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NotikaEmail_IDMastery.Context;
using NotikaEmail_IDMastery.Entities;

namespace NotikaEmail_IDMastery.ViewComponents
{
     public class _HeaderAdminLayoutComponentPartial : ViewComponent
    {
        private readonly EmailContext _context;
        private readonly UserManager<AppUser> _userManager;

        public _HeaderAdminLayoutComponentPartial(EmailContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task <IViewComponentResult> InvokeAsync()
        {
            var userValue = await _userManager.FindByNameAsync(User.Identity.Name);
            var userEmail = userValue.Email;
            var userEmailCount = _context.Messages.Where(x => x.ReceiverEmail == userEmail).Count();
            ViewBag.userEmailCount = userEmailCount;
            return View();
        }
    }
}
