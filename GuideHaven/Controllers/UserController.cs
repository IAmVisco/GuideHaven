﻿using System;
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

        [HttpGet("User/{name}")]
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
            context.Database.ExecuteSqlCommand("DBCC CHECKIDENT('dbo.Medals', RESEED, 0)");

            context.Medals.Add(new Medal() { Name = "1stLike", Description = "1stLikeDesc", Image = "https://ucarecdn.com/9cfce6f9-10c5-480d-911f-d6c80f5669aa/like.png" });
            context.Medals.Add(new Medal() { Name = "10Likes", Description = "10LikesDesc", Image = "https://ucarecdn.com/d34459e1-8807-4d39-a3e9-0e11fbb3ca85/likered.png" });
            //context.Medals.Add(new Medal() { Name = "100 likes", Description = "Like 100 comments" });

            context.Medals.Add(new Medal() { Name = "1stComment", Description = "1stCommentDesc", Image = "https://ucarecdn.com/b890b7e8-2b37-4a31-9d78-e4849ea72fb6/comment.png" });
            context.Medals.Add(new Medal() { Name = "10Comments", Description = "10CommentsDesc", Image = "https://ucarecdn.com/483398a3-5cc2-40f9-aa67-e5dbb203ebf9/commentgold.png" });
            //context.Medals.Add(new Medal() { Name = "100 comments", Description = "Leave 100 comments to any guides" });

            context.Medals.Add(new Medal() { Name = "NewbieCritic", Description = "NewbieCriticDesc", Image = "https://ucarecdn.com/1b7134a3-4846-47e6-b3b1-ad9dcc3ea1e3/critic.png" });
            context.Medals.Add(new Medal() { Name = "ExpCritic", Description = "ExpCriticDesc", Image = "https://ucarecdn.com/800e6cca-7865-4f79-8b32-d251f6bcea5e/criticgold.png" });
            //context.Medals.Add(new Medal() { Name = "Senior critic", Description = "Rate 100 guides" });

            context.Medals.Add(new Medal() { Name = "1stGuide", Description = "1stGuideDesc", Image = "https://ucarecdn.com/b141616d-ec03-4f91-bd7d-5b3c14002896/firstguide.png" });
            context.Medals.Add(new Medal() { Name = "5Star", Description = "5StarDesc", Image = "https://ucarecdn.com/cd0f42f1-4f74-4536-b883-e05a293db0de/5star.png" });

            context.Medals.Add(new Medal() { Name = "MadLad", Description = "MadLadDesc", Image = "https://ucarecdn.com/b074ad4d-1353-4ae5-beb3-ffc8c8aa2c1d/madlad.png" });
            context.SaveChanges();
        }
    }
}