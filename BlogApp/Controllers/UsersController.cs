using System.Security.Claims;
using BlogApp.Models;
using BlogApp.Services.Abstract;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Index", "Posts");
            }
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var success = await _userService.RegisterAsync(model);
                if (success)
                {
                    return RedirectToAction("Login");
                }

                ModelState.AddModelError("", "Username or Email is already in use.");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userService.AuthenticateAsync(model.Email, model.Password);

                if (user != null)
                {                
                    var userClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                        new Claim(ClaimTypes.Name, user.UserName ?? ""),
                        new Claim(ClaimTypes.GivenName, user.Name ?? ""),
                        new Claim(ClaimTypes.UserData, user.Image ?? "")
                    };

                    if (user.Email == "berkayysimsekk@gmail.com")
                    {
                        userClaims.Add(new Claim(ClaimTypes.Role, "admin"));
                    }

                    var claimsIdentity = new ClaimsIdentity(
                        userClaims,
                        CookieAuthenticationDefaults.AuthenticationScheme
                    );

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true
                    };

                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties
                    );

                    return RedirectToAction("Index", "Posts");
                }

                ModelState.AddModelError("", "Kullanıcı adı veya şifre yanlış.");
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Profile(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return NotFound();
            }

            var user = await _userService.GetUserProfileAsync(username);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
    }
}
