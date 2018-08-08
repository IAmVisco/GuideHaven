using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GuideHaven.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

namespace GuideHaven.Areas.Identity.Pages.Account.Manage
{
    public class GuideManagementModel : PageModel
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IEmailSender emailSender;
        private readonly IStringLocalizer<IdentityLocalizer> localizer;
        private readonly GuideContext context;

        public GuideManagementModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IEmailSender emailSender,
            IStringLocalizer<IdentityLocalizer> localizer,
            GuideContext context)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailSender = emailSender;
            this.localizer = localizer;
            this.context = context;
        }

        public List<Guide> Guides { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Guides = context.GetGuides(context, User.FindFirstValue(ClaimTypes.NameIdentifier));
            return Page();
        }
    }
}