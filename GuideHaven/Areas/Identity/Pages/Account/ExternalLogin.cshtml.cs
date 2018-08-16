using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using GuideHaven.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace GuideHaven.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ExternalLoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<ExternalLoginModel> logger;
        private readonly IEmailSender emailSender;
        private readonly IStringLocalizer<IdentityLocalizer> localizer;

        public ExternalLoginModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<ExternalLoginModel> logger,
            IEmailSender emailSender,
            IStringLocalizer<IdentityLocalizer> localizer)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.logger = logger;
            this.emailSender = emailSender;
            this.localizer = localizer;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string LoginProvider { get; set; }

        public bool IsEmailRequired { get; set; }

        public string ReturnUrl { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Username")]
            public string Login { get; set; }

            [EmailAddress]
            public string Email { get; set; }
        }

        public IActionResult OnGetAsync()
        {
            return RedirectToPage("./Login");
        }

        public IActionResult OnPost(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (remoteError != null)
            {
                ErrorMessage = localizer["ProviderError"] + remoteError;
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }
            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {         
                ErrorMessage = localizer["ExternalLoginError"];
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                if (info.LoginProvider != "Twitter" && info.LoginProvider != "Vkontakte" && await userManager.IsInRoleAsync(userManager.Users.First(x => x.Email == info.Principal.FindFirstValue(ClaimTypes.Email)), "Banned"))
                {
                    await signInManager.SignOutAsync();
                    ErrorMessage = localizer["Banned"];
                    return RedirectToPage("./Login");
                }
                logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);
                return LocalRedirect(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return RedirectToPage("./Lockout");
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ReturnUrl = returnUrl;
                LoginProvider = info.LoginProvider;
                IsEmailRequired = info.Principal.FindFirstValue(ClaimTypes.Email) == null;
                return Page();
            }
        }

        public async Task<IActionResult> OnPostConfirmationAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            // Get the information about the user from the external login provider
            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = localizer["ExternalLoginConfError"];
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            if (ModelState.IsValid)
            {
                ApplicationUser user = null;
                if (info.Principal.FindFirstValue(ClaimTypes.Email) is null)
                {
                    user = new ApplicationUser { UserName = Input.Login, Email =  Input.Email };
                }
                else
                {
                    user = new ApplicationUser { UserName = Input.Login, Email = info.Principal.FindFirstValue(ClaimTypes.Email) };
                    user.EmailConfirmed = true;
                }
                var result = await userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        if (user.EmailConfirmed)
                        {
                            await signInManager.SignInAsync(user, isPersistent: false);
                            logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
                            return LocalRedirect(returnUrl);
                        }
                        else
                        {
                            var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
                            var callbackUrl = Url.Page(
                                "/Account/ConfirmEmail",
                                pageHandler: null,
                                values: new { userId = user.Id, code = code },
                                protocol: Request.Scheme);
                            await emailSender.SendEmailAsync(Input.Email, localizer["ConfEmail"],
                            localizer["PlsConfirm"] + $" <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>" + localizer["ClickingHere"] + "</a>.");

                            return RedirectToPage("./RegisterSuccess");
                        }
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            LoginProvider = info.LoginProvider;
            ReturnUrl = returnUrl;
            IsEmailRequired = info.Principal.FindFirstValue(ClaimTypes.Email) == null;
            return Page();
        }
    }
}
