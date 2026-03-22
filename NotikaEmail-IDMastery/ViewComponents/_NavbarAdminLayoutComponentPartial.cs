using Microsoft.AspNetCore.Mvc;

namespace NotikaEmail_IDMastery.ViewComponents
{
    public class _NavbarAdminLayoutComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
