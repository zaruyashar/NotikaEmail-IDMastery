using Microsoft.AspNetCore.Mvc;

namespace NotikaEmail_IDMastery.ViewComponents.MessageViewComponents
{
    public class _MessageSidebarComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
