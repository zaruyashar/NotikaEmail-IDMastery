using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NotikaEmail_IDMastery.Context;
using NotikaEmail_IDMastery.Entities;
using NotikaEmail_IDMastery.Models.MessageViewModels;

namespace NotikaEmail_IDMastery.ViewComponents.NavbarHeadViewComponents
{
    public class _MessageListNavbarDropdownComponentPartial : ViewComponent
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly EmailContext _context;

        public _MessageListNavbarDropdownComponentPartial(UserManager<AppUser> userManager, EmailContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userValue = await _userManager.FindByNameAsync(User.Identity.Name);
            var userEmail = userValue.Email;
            var values = from message in _context.Messages
                         join user in _context.Users
                         on message.SenderEmail equals user.Email
                         where message.ReceiverEmail == user.Email
                         select new MessageListWithUserInfoViewModel
                         {
                             FullName = user.Name + " " + user.Surname,
                             ProfileImageUrl = user.ImageUrl,
                             SendDate = message.SendDate,
                             MessageDetail = message.MessageDetails
                         };
            return View(values.ToList());
        }
    }
}
