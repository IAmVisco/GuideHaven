using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GuideHaven.Models;
using Microsoft.AspNetCore.Identity;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity.UI.Services;
using GuideHaven.Areas.Identity.Data;

namespace GuideHaven.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IServiceProvider serviceProvider;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IStringLocalizer<HomeController> localizer;
        private readonly IEmailSender emailSender;
        private readonly GuideContext context;

        private List<Guide> Guides { get; set; }

        public HomeController(UserManager<ApplicationUser> userManager, IServiceProvider serviceProvider, 
            SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, IEmailSender emailSender,
            IStringLocalizer<HomeController> localizer, GuideContext context)
        {
            this.userManager = userManager;
            this.serviceProvider = serviceProvider;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.localizer = localizer;
            this.emailSender = emailSender;
            this.context = context;
        }

        public class InputModel
        {
            [Required]
            [StringLength(32, ErrorMessage = "LengthWarning", MinimumLength = 4)]
            [DataType(DataType.Text)]
            [Display(Name = "Username")]
            public string UserName { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "LengthWarning", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "ConfPass")]
            [Compare("Password", ErrorMessage = "PassMismatch")]
            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> Index()
        {
            return View(await context.Guide.Include(x => x.Ratings).Include(x => x.Comments).OrderByDescending(x => x.GuideId).ToListAsync());
        }

        //public async Task<IActionResult> Index()
        //{
        //    await CreateUserRoles();
        //    return View();
        //}

        public IActionResult About()
        {
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

        [HttpPost]
        public async Task<IActionResult> Index(InputModel Input)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = Input.UserName, Email = Input.Email };
                var result = await userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Action(
                        "ConfirmEmail",
                        "Identity/Account",
                        values: new { userId = user.Id, code = code },
                        protocol: Request.Scheme);

                    //callbackUrl.Insert(callbackUrl.IndexOf('?'), "Identity/Account/ConfirmEmail");

                    await emailSender.SendEmailAsync(Input.Email, localizer["ConfEmail"],
                        localizer["PlsConfirm"] + $" <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>" + localizer["ClickingHere"] + "</a>.");

                    // await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("RegisterSuccess","Identity/Account");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return RedirectToAction("Index");
        }
    }
}
