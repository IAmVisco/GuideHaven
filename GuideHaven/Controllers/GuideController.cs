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
        private readonly IdentityContext identityContext;

        public GuideController(GuideContext context, UserManager<ApplicationUser> userManager, IdentityContext identityContext)
        {
            this.context = context;
            this.userManager = userManager;
            this.identityContext = identityContext;
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
                var list1 = context.Guide.Include(g => g.GuideTags).Include(g => g.GuideSteps).FromSql($"select * FROM Guide WHERE FREETEXT( * , {searchText})").ToList();
                var list2 = context.Steps.Include(x => x.Guide).FromSql($"select * FROM Step WHERE FREETEXT( * , {searchText})").Select(x => x.Guide).ToList();
                return View("Index", list1.Union(list2).ToList());
            }
            else
                return View("Index", await context.Guide.ToListAsync());
        }

        [AllowAnonymous]
        [HttpPost("Tags/{searchText}")]
        [HttpGet("Tags/{searchText}")]
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

        [AllowAnonymous]
        [HttpGet("Categories/{category}")]
        public async Task<IActionResult> GetGuidesByCategory(string category)
        {
            var guides = context.GetGuides(context);
            if (!string.IsNullOrEmpty(category))
            {
                return View("Index", guides.Where(x => x.Category == category).ToList());
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
                    context.SaveTags(context, tagsList, null);
                }
                guide.GuideSteps.RemoveAll(x => x.Header == null && x.Content == null);
                guide.Owner = await userManager.GetUserIdAsync(await userManager.GetUserAsync(User));
                guide.CreationDate = DateTime.Now;
                context.Add(guide);
                await context.SaveChangesAsync();
                await CheckMedals(new int[] { 1 }, 7, 7, context.Guide.Where(x => x.Owner == guide.Owner).ToList());
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
                        context.SaveTags(context, tagsList, id);
                    }
                    guide.GuideSteps.RemoveAll(x => x.Header == null && x.Content == null);
                    context.Steps.RemoveRange(context.Steps.Where(x => x.GuideId == guide.GuideId));
                    context.Update(guide);
                    if (tags == null)
                        context.Guide.Include(x => x.GuideTags).FirstOrDefault(x => x.GuideId == id).GuideTags.Clear();
                    else
                        context.Guide.Include(x => x.GuideTags).FirstOrDefault(x => x.GuideId == id).GuideTags.RemoveAll(x => !TagListCreator(tags).Any(t => t.TagId == x.TagId));
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
        public async Task<ActionResult<string>> PostComment(int guideId, string comment)
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
            string output = CreateComment(context.GetGuide(context, guideId).Comments.Last());
            await CheckMedals(new int[] { 1, 10 }, 3, 4, context.Comments.Where(x => x.Owner == User.Identity.Name).ToList());
            return output;
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
                output += CreateComment(item);
            }
            return output;
        }

        [AllowAnonymous]
        public ActionResult<int[]> GetLikes(int guideId)
        {
            var guide = context.GetGuide(context, guideId);
            List<int> likes = new List<int>();
            foreach (var item in guide.Comments)
            {
                likes.Add(item.Likes.Count);
            }
            return likes.ToArray();
        }

        [HttpPost]
        public async Task<IActionResult> PostRating(int guideId, int rating)
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
            await CheckRateMedals(rating);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> PostLike(int guideId, int commentId)
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
            await CheckMedals(new int[] { 1, 10 }, 1, 2, context.Likes.Where(x => x.Owner == User.Identity.Name).ToList());
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
                GuideTag guideTag = new GuideTag() { Guide = guide, TagId = item.TagId, Tag = item, GuideId = guide.GuideId };
                list.Add(guideTag);
                item.GuideTags = new List<GuideTag>();
                item.GuideTags.Add(guideTag);
            }
            //guide.GuideTags = list;
        }

        private async Task CheckMedals<T>(int[] conditions, int startMedalIndex, int endMedalIndex, List<T> list)
        {       
            var user = identityContext.Users.Include(x => x.Medals).FirstOrDefault(x => x.UserName == User.Identity.Name);
            CheckProcess(user, conditions, startMedalIndex, endMedalIndex, list);
            await CheckSuperMedal();
            identityContext.SaveChanges();
        }        

        private void CheckProcess<T>(ApplicationUser user, int[] conditions, int startMedalIndex, int endIndex, List<T> list)
        {
            int index = 0;
            for (int i = startMedalIndex; i <= endIndex; i++)
            {
                if (list.Count() >= conditions[index] && !user.Medals.Any(x => x.MedalId == i))
                {
                    AddMedal(i, user);
                }
                index++;
            }
        }

        private async Task CheckRateMedals(int rating)
        {
            var user = identityContext.Users.Include(x => x.Medals).FirstOrDefault(x => x.UserName == User.Identity.Name);
            if (rating == 5 && !user.Medals.Any(x => x.MedalId == 5))
            {
                AddMedal(5, user);
            }
            if (context.Ratings.Where(x => x.Owner == User.Identity.Name).Count() >= 1 && !user.Medals.Any(x => x.MedalId == 6))
            {
                AddMedal(6, user);
            }
            await CheckSuperMedal();
            identityContext.SaveChanges();
        }

        private async Task CheckSuperMedal()
        {
            var user = await userManager.GetUserAsync(User);
            if (user.Medals.Count == identityContext.Medals.Count() - 1)
                AddMedal(identityContext.Medals.Count(), user);
        }

        private void AddMedal(int id, ApplicationUser user)
        {
            var newMedal = new AspNetUserMedals() { Medal = identityContext.Medals.FirstOrDefault(x => x.Id == id), MedalId = id, User = user, UserId = user.Id };
            identityContext.Medals.Include(x => x.Users).FirstOrDefault(x => x.Id == id).Users.Add(newMedal);
        }

        private string CreateComment(Comment item)
        {
            string liked = "";
            if (item.Likes.FirstOrDefault(x => x.Owner == User.Identity.Name) != null)
                liked += " checked ";
            return "<label class=\"commenter\">" + item.Owner + ":</label>"
                    + "<div class=\"comment-wrap\">"
                        + "<div class=\"comment-block\">"
                            + "<input id=\"commentId\" hidden value=" + item.CommentId + " />"
                            + "<p>" + item.Content + "</p>"
                            + "<div class=\"bottom-comment\">"
                                + "<div class=\"comment-date\">" + item.CreationTime.ToString("HH:mm:ss dd.MM.yyyy") + "</div>"
                                + "<div class=\"comment-actions\">"
                                    + "<input" + liked + " type = \"checkbox\" class=\"like-btn\" id=\"like-" + item.CommentId + "\" value=\"" + item.CommentId + "\"/>"
                                    + "<label for=\"like-" + item.CommentId + "\" value=\"" + item.CommentId + "\" class=\"like-lbl\" title=\"Like!\"></label>"
                                    + "<span class=\"like-count\">" + item.Likes.Count + "</span>"
                                + "</div>"
                            + "</div>"
                        + "</div>"
                    + "</div>";
        }
    }
}
