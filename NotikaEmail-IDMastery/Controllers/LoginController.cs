using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NotikaEmail_IDMastery.Context;
using NotikaEmail_IDMastery.Entities;
using NotikaEmail_IDMastery.Models.IdentityModels;
using NotikaEmail_IDMastery.Models.JWTModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NotikaEmail_IDMastery.Controllers
{
    public class LoginController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly EmailContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly JwtSettingsModel _jwtSettingsModel;

        public LoginController(SignInManager<AppUser> signInManager, EmailContext context, UserManager<AppUser> userManager, IOptions<JwtSettingsModel> jwtSettingsModel)
        {
            _signInManager = signInManager;
            _context = context;
            _userManager = userManager;
            _jwtSettingsModel = jwtSettingsModel.Value;
        }

        [HttpGet]
        public IActionResult UserLogin()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UserLogin(UserLoginViewModel model)
        {
            var value = _context.Users.Where(x => x.UserName == model.Username).FirstOrDefault();

            if (value == null)
            {
                ModelState.AddModelError(string.Empty, "Girdiğiniz kullanıcı adı sistemde bulunamadı.");
                return View(model);
            }

            SimpleUserViewModel simpleUserViewModel = new SimpleUserViewModel()
            {
                City = value.City,
                Email = value.Email,
                Id = value.Id,
                Name = value.Name,
                Surname = value.Surname,
                Username = value.UserName
            };

            if (!value.EmailConfirmed)
            {
                ModelState.AddModelError(string.Empty, "E-pota adresinizi henüz onaylamamışsınız.");
                return View(model);
            }

            if (!value.IsActive)
            {
                ModelState.AddModelError(string.Empty, "Kullanıcı henüz aktifleştirilmediğinden giriş yapamaz.");
                return View(model);
            }

            if (value.EmailConfirmed)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, true);

                if (result.Succeeded)
                {
                    var token = GenerateJwtToken(simpleUserViewModel);
                    Response.Cookies.Append("jwtToken", token, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTime.UtcNow.AddMinutes(_jwtSettingsModel.ExpireMinutes)
                    });
                    return RedirectToAction("EditProfile", "Profile");
                }

                ModelState.AddModelError(string.Empty, "Kullanıcı adı veya şifre hatalı.");
                return View(model);
            }

            ModelState.AddModelError(string.Empty, "Lütfen giriş yapmadan önce e-posta adresinizi doğrulayın.");
            return View();
        }


        public string GenerateJwtToken(SimpleUserViewModel simpleUserViewModel)
        {
            var claim = new[]
            {
                new Claim("name", simpleUserViewModel.Name ?? ""),
                new Claim("surname", simpleUserViewModel.Surname ?? ""),
                new Claim("city", simpleUserViewModel.City ?? ""),
                new Claim("username", simpleUserViewModel.Username ?? ""),
                new Claim(ClaimTypes.NameIdentifier, simpleUserViewModel.Id ?? ""),
                new Claim(ClaimTypes.Email, simpleUserViewModel.Email ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettingsModel.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettingsModel.Issuer,
                audience: _jwtSettingsModel.Audience,
                claims: claim,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettingsModel.ExpireMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        [HttpGet]
        public IActionResult LoginWithGoogle()
        {
            return View();
        }

        // redirect to social media provider
        [HttpPost]
        public IActionResult ExternalLogin(string provider, string? returnUrl = null)
        {
            var redirectUrl = Url.Action("ExternalLoginCallBack", "Login", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        //
        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallBack(string? returnUrl = null, string? remoteError = null)
        {
            returnUrl ??= Url.Content("~/");
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"External Provider Error: {remoteError}");
                return RedirectToAction("UserLogin");
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction("UserLogin");
            }
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
            if (result.Succeeded)
            {
                return RedirectToAction("Inbox", "Message");
            }

            // if no user, create a new one
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var user = new AppUser()
            {
                UserName = email,
                Email = email,
                Name = info.Principal.FindFirstValue(ClaimTypes.GivenName) ?? "Google",
                Surname = info.Principal.FindFirstValue(ClaimTypes.Surname) ?? "User"
            };

            var identityResult = await _userManager.CreateAsync(user);
            if (identityResult.Succeeded)
            {
                await _userManager.AddLoginAsync(user, info);
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Inbox", "Message");
            }
            return RedirectToAction("UserLogin");
        }
    }
}
