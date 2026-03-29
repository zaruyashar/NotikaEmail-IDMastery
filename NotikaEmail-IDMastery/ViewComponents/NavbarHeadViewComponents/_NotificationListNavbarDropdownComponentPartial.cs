using Microsoft.AspNetCore.Mvc;
using NotikaEmail_IDMastery.Context;

namespace NotikaEmail_IDMastery.ViewComponents.NavbarHeadViewComponents
{
    public class _NotificationListNavbarDropdownComponentPartial : ViewComponent
    {
        private readonly EmailContext _context;

        public _NotificationListNavbarDropdownComponentPartial(EmailContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var values = _context.Notifications.ToList();
            return View(values);
        }
    }
}
