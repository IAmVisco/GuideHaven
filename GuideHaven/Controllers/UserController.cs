using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GuideHaven.Areas.Identity.Data;
using GuideHaven.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace GuideHaven.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IServiceProvider serviceProvider;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IStringLocalizer<UserController> localizer;
        private readonly IdentityContext context;
        private readonly GuideContext guidecontext;

        public UserController(UserManager<ApplicationUser> userManager, IServiceProvider serviceProvider,
            SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager,
            IStringLocalizer<UserController> localizer, IdentityContext context, GuideContext guidecontext)
        {
            this.userManager = userManager;
            this.serviceProvider = serviceProvider;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.localizer = localizer;
            this.context = context;
            this.guidecontext = guidecontext;
        }

        public async Task<IActionResult> Index(string name)
        {
            ViewData["Guides"] = guidecontext.GetGuides(guidecontext, userManager.Users.FirstOrDefault(x => x.UserName == name).Id);
            return View(await userManager.FindByNameAsync(name));
        }
    }
}