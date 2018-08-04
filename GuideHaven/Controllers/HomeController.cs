using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GuideHaven.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace GuideHaven.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IServiceProvider serviceProvider;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public HomeController(UserManager<IdentityUser> userManager, IServiceProvider serviceProvider, 
            SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.serviceProvider = serviceProvider;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View();
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

        //[Authorize(Roles = "Admin")] does not work
        public IActionResult AdminPanel()
        {
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
