using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Localization;

namespace GuideHaven.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly ILogger<LoginModel> logger;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IStringLocalizer<IdentityLocalizer> localizer;

        public LoginModel(SignInManager<IdentityUser> signInManager, 
            ILogger<LoginModel> logger, 
            UserManager<IdentityUser> userManager,
            IStringLocalizer<IdentityLocalizer> localizer)
        {
            this.signInManager = signInManager;
            this.logger = logger;
            this.userManager = userManager;
            this.localizer = localizer;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            //[Required]
            //[EmailAddress]
            //public string Email { get; set; }

            [Required]
            [StringLength(32, ErrorMessage = "LengthWarning", MinimumLength = 6)]
            [DataType(DataType.Text)]
            [Display(Name = "Username")]
            public string UserName { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
                var result = await signInManager.PasswordSignInAsync(Input.UserName, Input.Password, Input.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded && await userManager.IsInRoleAsync(userManager.Users.First(x => x.UserName == Input.UserName), "Banned"))
                {
                    ModelState.AddModelError(string.Empty, localizer["Banned"]);
                    await signInManager.SignOutAsync();
                    return Page();
                }
                if (!await userManager.IsEmailConfirmedAsync(userManager.Users.First(x => x.UserName == Input.UserName)))
                {
                    ModelState.AddModelError(string.Empty, localizer["MustConfEmail"]);
                    //await _signInManager.SignOutAsync();
                    return Page();
                }
                if (result.Succeeded)
                {
                    logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, localizer["InvalidLogin"]);
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
