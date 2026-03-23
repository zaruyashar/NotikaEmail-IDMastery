using Microsoft.AspNetCore.Mvc;
using NotikaEmail_IDMastery.Context;

namespace NotikaEmail_IDMastery.ViewComponents.MessageViewComponents
{
    public class _MessageCategoryListSidebardComponentPartial : ViewComponent
    {
        private readonly EmailContext _context;

        public _MessageCategoryListSidebardComponentPartial(EmailContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var values = _context.Categories.ToList();
            return View(values);
        }
    }
}
