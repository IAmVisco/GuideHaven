using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GuideHaven.Areas.Identity.Data;
using Rotativa.AspNetCore;
using PagedList.Core;

namespace GuideHaven.Models
{
    [Authorize]
    public class GuideController : Controller
    {
        public int PageSize { get; set; } = 10;

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
        public IActionResult Index(string searchText, string tag, string category, int page = 1)
        {
            var guides = CheckInput(searchText, tag, category);
            return View(guides.OrderByDescending(x => x.GuideId).ToPagedList(page, PageSize));
        }

        public IQueryable<Guide> Search(string searchText)
        {
            if (!string.IsNullOrEmpty(searchText))
            {
                var list1 = context.Guide.Include(x => x.Ratings).Include(g => g.GuideSteps).FromSql($"select * FROM Guide WHERE FREETEXT( * , {searchText})").ToList();
                var list2 = context.Steps.Include(x => x.Guide).ThenInclude(guide => guide.Ratings).FromSql($"select * FROM Step WHERE FREETEXT( * , {searchText})").Select(x => x.Guide).ToList();
                return list1.Union(list2).AsQueryable();
            }
            else
                return context.Guide.Include(x => x.Ratings).AsQueryable();
        }

        public IQueryable<Guide> SearchTags(string tag)
        {
            return context.GetGuides(context).Where(x => x.GuideTags.Any(t => t.TagId == tag)).AsQueryable();
        }

        public IQueryable<Guide> GetGuidesByCategory(string category)
        {
            return context.Guide.Include(x => x.Ratings).FromSql($"select * FROM Guide WHERE FREETEXT( Category , {category})");
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

        [HttpPost]
        public IActionResult DeleteComment(int id, int guideId)
        {
            var guide = context.GetGuide(context, guideId);
            guide.Comments.RemoveAll(x => x.CommentId == id);
            context.SaveChanges();
            return Ok();
        }

        [AllowAnonymous]
        public IActionResult PDF(int? id)
        {
            return new ViewAsPdf("PDF", context.GetGuide(context, id));
            //return View(context.GetGuide(context, id));
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
                AddTags(guide, tags, null);
                guide.GuideSteps.RemoveAll(x => x.Header == null && x.Content == null);
                guide.Owner = (await userManager.GetUserAsync(User)).Id;
                guide.CreationDate = DateTime.Now;
                context.Add(guide);
                await context.SaveChangesAsync();
                await CheckMedals(new int[] { 1 }, 7, 7, context.GetGuides(context, guide.Owner));
                return RedirectToAction("Details", new { id = context.Guide.LastOrDefault(x => x.GuideName == guide.GuideName).GuideId });
            }
            return View(guide);
        }

        // GET: Guide/Edit/5
        public IActionResult Edit(int? id)
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
                    AddTags(guide, tags, id);
                    guide.GuideSteps.RemoveAll(x => x.Header == null && x.Content == null);
                    context.Steps.RemoveRange(context.Steps.Where(x => x.GuideId == guide.GuideId));
                    context.Update(guide);
                    if (string.IsNullOrEmpty(tags))
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
            string output = await CreateComment(context.GetGuide(context, guideId).Comments.Last());
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
        public async Task<ActionResult<string>> GetComments(int guideId)
        {
            string output = "";
            foreach (var item in context.GetGuide(context, guideId).Comments)
            {
                output += await CreateComment(item);
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
                guide.Ratings.FirstOrDefault(x => x.Owner == User.Identity.Name).OwnerRating = rating;
            }
            context.SaveChanges();
            await CheckMedals(new int[] { 1, 10 }, 5, 6, context.Ratings.Where(x => x.Owner == User.Identity.Name).ToList());
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
        public async Task<IActionResult> DeleteConfirmed(int? id, string returnUrl, string user)
        {
            returnUrl = returnUrl + ("?user=" + user ?? "") ?? Url.Content("~/");
            var guide = context.GetGuide(context, id);
            context.Guide.Remove(guide);
            await context.SaveChangesAsync();
            return LocalRedirect(returnUrl);
        }

        private bool GuideExists(int id)
        {
            return context.Guide.Any(e => e.GuideId == id);
        }

        private void AddTags(Guide guide, string tags, int? id)
        {
            if (!string.IsNullOrEmpty(tags))
            {
                var tagsList = TagListCreator(tags);
                CreateGuideTagsConnection(guide, tagsList);
                context.SaveTags(context, tagsList, id);
            }
        }

        private List<Tag> TagListCreator(string tags)
        {
            var tagsArray = tags.Split(',');
            List<Tag> tagsList = new List<Tag>();
            foreach (var item in tagsArray.Select(s => s.Trim().ToLower()))
            {
                if (item.Length <= 24)
                {
                    Tag tag = new Tag() { TagId = item };
                    tagsList.Add(tag);
                }
            }
            return tagsList;
        }

        private void CreateGuideTagsConnection(Guide guide, List<Tag> tags)
        {
            foreach (var item in tags)
            {
                GuideTag guideTag = new GuideTag() { Guide = guide, TagId = item.TagId, Tag = item, GuideId = guide.GuideId };
                item.GuideTags = new List<GuideTag>();
                item.GuideTags.Add(guideTag);
            }
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

        private IQueryable<Guide> CheckInput(string searchText, string tag, string category) {
            var guides = context.Guide.Include(x => x.Ratings).OrderByDescending(x => x.GuideId).AsQueryable(); ;
            if (!String.IsNullOrEmpty(searchText))
            {
                guides = Search(searchText);
                ViewBag.searchText = searchText;
            }
            else if (!String.IsNullOrEmpty(category))
            {
                guides = GetGuidesByCategory(category);
                ViewBag.category = category;
            }
            else if (!String.IsNullOrEmpty(tag))
            {
                guides = SearchTags(tag);
                ViewBag.tag = tag;
            }
            return guides;
        }

        private async Task<string> CreateComment(Comment item)
        {
            string liked = "", delete = "";
            if (item.Likes.FirstOrDefault(x => x.Owner == User.Identity.Name) != null)
                liked += " checked ";
            if (!User.Identity.IsAuthenticated)
                liked += " disabled ";
            if (item.Owner == userManager.GetUserName(User) ||
                (User.Identity.IsAuthenticated && await userManager.IsInRoleAsync(await userManager.GetUserAsync(User), "Admin")))
                delete = "<button href=\"#\" title=\"\" value=\"" + item.CommentId + "\" class=\"btn-link cmnt-delete\"><span class=\"glyphicon glyphicon-remove\"></span></button>";
            return "<div id=\""+ item.CommentId +"\"><label class=\"commenter\"><a href=\"../../User?user=" + item.Owner + "\">" + item.Owner + "</a>:</label>"
                    + "<div class=\"comment-wrap\">"
                        + delete
                        + "<div class=\"comment-block\">"
                            + "<input id=\"commentId\" hidden value=\"" + item.CommentId + "\" />"
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
                    + "</div></div>";
        }
    }
}
