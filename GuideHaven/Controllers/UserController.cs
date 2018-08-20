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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace GuideHaven.Controllers
{
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

        public class ProfileModel
        {
            public List<Medal> Medals { get; set; }
            public ApplicationUser User { get; set; }
            public List<Guide> Guides { get; set; }
        }

        public async Task<IActionResult> Index(string name)
        {
            //CreateMedals();
            var user = context.Users.FirstOrDefault(x => x.UserName == name);
            ProfileModel profile = new ProfileModel()
            {
                Guides = guidecontext.GetGuides(guidecontext, userManager.Users.FirstOrDefault(x => x.UserName == name).Id),
                User = user,
                Medals = context.Medals.Where(x => x.Users.Any(m => m.UserId == user.Id)).ToList()
            };
            return View(profile);
        }

        public void CreateMedals()
        {
            context.Medals.Add(new Medal() { Name = "First like", Description = "Post your first like to any comment" });
            context.Medals.Add(new Medal() { Name = "10 likes", Description = "Post 10 likes to any comments" });
            context.Medals.Add(new Medal() { Name = "100 likes", Description = "Post 100 likes to any comments" });
            context.Medals.Add(new Medal() { Name = "First comment", Description = "Leave your first comment" });
            context.Medals.Add(new Medal() { Name = "10 comments", Description = "Post 10 comments to any guides" });
            context.Medals.Add(new Medal() { Name = "100 comments", Description = "Post 100 comments to any guides" });
            context.Medals.Add(new Medal() { Name = "First guide", Description = "Create your first guide" });
            context.Medals.Add(new Medal() { Name = "5 Rating", Description = "Get 5 stars on any of your guides" });
            context.Medals.Add(new Medal() { Name = "Critic", Description = "Rate any guide" });
            context.SaveChanges();
        }
    }
}