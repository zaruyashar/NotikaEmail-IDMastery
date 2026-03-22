using Microsoft.AspNetCore.Mvc;

namespace NotikaEmail_IDMastery.ViewComponents
{
    public class _FooterAdminLayoutComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
