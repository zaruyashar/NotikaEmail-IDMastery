using Microsoft.AspNetCore.Mvc;

namespace NotikaEmail_IDMastery.ViewComponents
{
    public class _BreadcombAdminLayoutComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
