using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GuideHaven.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http;

namespace GuideHaven.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IServiceProvider serviceProvider;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IStringLocalizer<HomeController> localizer;

        public HomeController(UserManager<IdentityUser> userManager, IServiceProvider serviceProvider, 
            SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager, IStringLocalizer<HomeController> localizer)
        {
            this.userManager = userManager;
            this.serviceProvider = serviceProvider;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.localizer = localizer;
        }

        public IActionResult Index()
        {
            //ViewData["Title"] = localizer["Title"];
            return View();
        }

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }

        //public async Task<IActionResult> Index()
        //{
        //    await CreateUserRoles();
        //    return View();
        //}

        private async Task CreateUserRoles()
        {
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole()
                {
                    Name = "Admin"
                });
            }

            if (!await roleManager.RoleExistsAsync("Banned"))
            {
                await roleManager.CreateAsync(new IdentityRole()
                {
                    Name = "Banned"
                });
            }
        }

        public async Task <IActionResult> AdminPanel()
        {
            if (!signInManager.IsSignedIn(User) || !(await userManager.IsInRoleAsync(userManager.Users.First(x => x.UserName == User.Identity.Name), "Admin")))
                return RedirectToAction("AccessDenied", "Identity/Account");
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("Error/404")]
        public IActionResult Error404()
        {
            return View();
        }

        public async Task<IActionResult> AddRole(string[] list, string role)
        {
            foreach (var item in list)
            {
                if (!(await userManager.IsInRoleAsync(userManager.Users.First(x => x.UserName == item), role)))
                {
                    await userManager.AddToRoleAsync(userManager.Users.First(x => x.UserName == item), role);
                }
            } 
            if (list.Contains(userManager.GetUserName(User)) && role == "Banned")
            {
                await signInManager.SignOutAsync();
                return RedirectToAction("Login", "Identity/Account");
            }
            return RedirectToAction("AdminPanel");
        }

        public async Task<IActionResult> RemoveRole(string[] list, string role)
        {
            foreach (var item in list)
            {
                if (await userManager.IsInRoleAsync(userManager.Users.First(x => x.UserName == item), role))
                {
                    await userManager.RemoveFromRoleAsync(userManager.Users.First(x => x.UserName == item), role);
                }
            }
            if (list.Contains(userManager.GetUserName(User)) && role == "Admin")
                return RedirectToAction("Index", "Home");
            return RedirectToAction("AdminPanel");
        }

        public async Task<IActionResult> DeleteUsers(string[] list)
        {
            foreach (var item in list)
            {
                await userManager.DeleteAsync(userManager.Users.First(x => x.UserName == item));
            }
            if (list.Contains(userManager.GetUserName(User)))
            {
                await signInManager.SignOutAsync();
                return RedirectToAction("Login", "Identity/Account");
            }
            return RedirectToAction("AdminPanel");
        }
    }
}
