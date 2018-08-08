using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GuideHaven.Models
{
    [Authorize]
    public class GuideController : Controller
    {
        private readonly GuideContext context;
        private readonly UserManager<IdentityUser> userManager;

        public GuideController(GuideContext context, UserManager<IdentityUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        public string ReturnUrl { get; set; }

        // GET: Guide
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return View(await context.Guide.ToListAsync());
        }

        // GET: Guide/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var guide = context.GetGuide(context, id);
            if (guide == null)
            {
                return NotFound();
            }

            return View(guide);
        }

        // GET: Guide/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Guide/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GuideId,GuideName,GuideSteps")] Guide guide)
        {
            if (ModelState.IsValid)
            {
                guide.GuideSteps.RemoveAll(x => x.Header == null && x.Content == null);
                guide.Owner = await userManager.GetUserIdAsync(await userManager.GetUserAsync(User));
                context.Add(guide);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(guide);
        }

        // GET: Guide/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var guide = context.GetGuide(context, id);
            if (guide == null)
            {
                return NotFound();
            }
            return View(guide);
        }

        // POST: Guide/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("GuideId, GuideName, GuideSteps")] Guide guide)
        {
            if (id != guide.GuideId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    guide.GuideSteps.RemoveAll(x => x.Header == null && x.Content == null);
                    context.Steps.RemoveRange(context.Steps.Where(x => x.GuideId == guide.GuideId));
                    context.Update(guide);
                    await context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GuideExists(guide.GuideId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(guide);
        }

        // POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id, string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            var guide = context.GetGuide(context, id);
            context.Guide.Remove(guide);
            await context.SaveChangesAsync();
            return LocalRedirect(returnUrl);
            //return RedirectToAction(nameof(Index));
        }

        private bool GuideExists(int id)
        {
            return context.Guide.Any(e => e.GuideId == id);
        }
    }
}
