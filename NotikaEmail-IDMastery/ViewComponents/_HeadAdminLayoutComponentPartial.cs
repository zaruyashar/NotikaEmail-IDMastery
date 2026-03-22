using Microsoft.AspNetCore.Mvc;

namespace NotikaEmail_IDMastery.ViewComponents
{
    public class _HeadAdminLayoutComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
