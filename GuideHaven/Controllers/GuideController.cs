using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Korzh.EasyQuery.Linq;
using GuideHaven.Areas.Identity.Data;

namespace GuideHaven.Models
{
    [Authorize]
    public class GuideController : Controller
    {
        private readonly GuideContext context;
        private readonly UserManager<ApplicationUser> userManager;

        public GuideController(GuideContext context, UserManager<ApplicationUser> userManager)
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

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Search(string searchText)
        {
            if (!string.IsNullOrEmpty(searchText))
            {
                return View("Index", context.Guide.Include(g => g.GuideTags).FullTextSearchQuery(searchText, new FullTextSearchOptions() { }));
            }
            else
                return View("Index", await context.Guide.ToListAsync());
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SearchTags(string searchText)
        {
            var guides = context.GetGuides(context);
            if (!string.IsNullOrEmpty(searchText))
            {
                return View("Index", guides.Where(x => x.GuideTags.Any(t => t.TagId == searchText)).ToList());
            }
            else
                return View("Index");
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
            context.Guide.FirstOrDefault(x => x.GuideId == id).Views++;
            await context.SaveChangesAsync();
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
        public async Task<IActionResult> Create([Bind("GuideId, GuideName, GuideSteps, Description, Image, Tags, Category")] Guide guide, string tags = null)
        {
            if (ModelState.IsValid)
            {
                if (tags != null)
                {
                    var tagsList = TagListCreator(tags);
                    CreateGuideTagsConnection(guide, tagsList);
                    context.SaveTags(context, tagsList);
                }
                guide.GuideSteps.RemoveAll(x => x.Header == null && x.Content == null);
                guide.Owner = await userManager.GetUserIdAsync(await userManager.GetUserAsync(User));
                guide.CreationDate = DateTime.Now;
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
        public async Task<IActionResult> Edit(int id, 
            [Bind("GuideId, GuideName, GuideSteps, Owner, Description, Image, Views, CreationDate, Category")] Guide guide, string tags = null)
        {
            if (id != guide.GuideId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (tags != null)
                    {
                        var tagsList = TagListCreator(tags);
                        CreateGuideTagsConnection(guide, tagsList);
                        context.SaveTags(context, tagsList);
                    }
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
                return RedirectToAction("Details", new { id });
            }
            return View(guide);
        }

        [HttpPost]
        public IActionResult PostComment(int guideId, string comment)
        {
            Comment newComment = new Comment()
            {
                Content = comment,
                CreationTime = DateTime.Now,
                Owner = User.Identity.Name
            };
            var guide = context.GetGuide(context, guideId);
            guide.Comments.Add(newComment);
            context.SaveChanges();
            return Ok();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult<double> GetRating(int guideId)
        {
            return context.GetGuide(context, guideId).GetRating();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult<string> GetComments(int guideId)
        {
            string output = "";
            foreach (var item in context.GetGuide(context, guideId).Comments)
            {
                string liked = "";
                if (item.Likes.FirstOrDefault(x => x.Owner == User.Identity.Name) != null)
                    liked += " checked ";
                output +=
                "<label class=\"commenter\">" + item.Owner + ":</label>"
                + "<div class=\"comment-wrap\">"
                    + "<div class=\"comment-block\">"
                        + "<input id=\"commentId\" hidden value=" + item.CommentId + " />"
                        + "<p>" + item.Content + "</p>"
                        + "<div class=\"bottom-comment\">"
                            + "<div class=\"comment-date\">" + item.CreationTime.ToString("HH:mm:ss dd.MM.yyyy") + "</div>"
                              + "<div class=\"comment-actions\">"
                                + "<input" + liked + " type = \"checkbox\" class=\"like-btn\" id=\"like-" + item.CommentId + "\" value=\"" + item.CommentId + "\"/>"
                                + "<label for=\"like-"+item.CommentId+ "\" value=\"" + item.CommentId + "\" class=\"like-lbl\" title=\"Like!\"></label>"
                                + "<span class=\"like-count\">" + item.Likes.Count + "</span>"
                            + "</div>"
                        + "</div>"
                    + "</div>"
                + "</div>";
            }
            return output;
        }

        [HttpPost]
        public IActionResult PostRating(int guideId, int rating)
        {
            var guide = context.GetGuide(context, guideId);
            Rating newRating = new Rating()
            {
                Owner = User.Identity.Name,
                GuideId = guideId,
                OwnerRating = rating,
                Guide = guide
            };
            if (guide.Ratings.FirstOrDefault(x => x.Owner == User.Identity.Name) == null)
            {
                guide.Ratings.Add(newRating);
            }
            else
            {
                context.Ratings.FirstOrDefault(x => x.Owner == User.Identity.Name).OwnerRating = rating;
            }
            context.SaveChanges();
            return Ok();
        }

        [HttpPost]
        public IActionResult PostLike(int guideId, int commentId)
        {
            var guide = context.GetGuide(context, guideId);
            var comment = guide.Comments.FirstOrDefault(x => x.CommentId == commentId);
            Like like = new Like()
            {
                Owner = User.Identity.Name,
                Comment = comment
            };
            if (comment.Likes.FirstOrDefault(x => x.Owner == User.Identity.Name) == null)
            {
                guide.Comments.Find(x => x == comment).Likes.Add(like);
            }
            else
            {
                guide.Comments.Find(x => x == comment).Likes.RemoveAll(g => g.Owner == User.Identity.Name);
            }
            context.SaveChanges();
            return Ok();
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

        private List<Tag> TagListCreator(string tags)
        {
            var tagsArray = tags.Split(',');
            List<Tag> tagsList = new List<Tag>();
            foreach (var item in tagsArray.Select(s => s.Trim().ToLower()))
            {
                Tag tag = new Tag() { TagId = item };
                tagsList.Add(tag);
            }
            return tagsList;
        }

        private void CreateGuideTagsConnection(Guide guide, List<Tag> tags)
        {
            List<GuideTag> list = new List<GuideTag>();
            foreach (var item in tags)
            {
                GuideTag guideTag = new GuideTag() { Guide = guide, TagId = item.TagId, Tag = item };
                list.Add(guideTag);
                item.GuideTags = new List<GuideTag>();
                item.GuideTags.Add(guideTag);
            }
            guide.GuideTags = list;
        }
    }
}
