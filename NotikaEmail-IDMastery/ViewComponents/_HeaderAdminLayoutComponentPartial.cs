using Microsoft.AspNetCore.Mvc;

namespace NotikaEmail_IDMastery.ViewComponents
{
    public class _HeaderAdminLayoutComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
