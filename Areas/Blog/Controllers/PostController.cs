using ASP_MVC.Areas.Blog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ASP_MVC.Models;
using ASP_MVC.Utilities;
using Microsoft.AspNetCore.Identity;

namespace ASP_MVC.Areas.Blog.Controllers
{
    [Area("Blog")]
    [Route("admin/blog/post/[action]/{id?}")]
    public class PostController : Controller
    {
        private readonly MasterDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public PostController(MasterDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [TempData] public string StatusMessage { get; set; }

        // GET: Blog/Post
        public async Task<IActionResult> Index([FromQuery(Name = "p")] int currentPage, int pageSize)
        {
            var posts = _context.Posts
                .Include(p => p.Author)
                .OrderByDescending(x => x.DateCreated);

            var totalPosts = await posts.CountAsync();
            if (pageSize <= 0) pageSize = 10;
            var countPages = (int)Math.Ceiling((double)totalPosts / pageSize);

            if (currentPage > countPages) currentPage = countPages;
            if (currentPage < 1) currentPage = 1;

            var pagingModel = new PagingModel()
            {
                countpages = countPages,
                currentpage = currentPage,
                generateUrl = pageNumber => Url.Action("Index", new
                {
                    p = pageNumber,
                    pagesize = pageSize
                })
            };

            ViewBag.pagingModel = pagingModel;
            ViewBag.totalPosts = totalPosts;

            ViewBag.postIndex = (currentPage - 1) * pageSize;

            var postsInPage = await posts.Skip((currentPage - 1) * pageSize)
                    .Take(pageSize)
                    .Include(p => p.Categories)
                    .ToListAsync()
                ;

            return View(postsInPage);

            /*
            model.totalUsers = await qr.CountAsync();
            model.countPages = (int)Math.Ceiling((double)model.totalUsers / model.ITEMS_PER_PAGE);

            if (model.currentPage < 1)
                model.currentPage = 1;
            if (model.currentPage > model.countPages)
                model.currentPage = model.countPages;

            var qr1 = qr.Skip((model.currentPage - 1) * model.ITEMS_PER_PAGE)
                .Take(model.ITEMS_PER_PAGE)
                .Select(u => new UserAndRole() {
                    Id = u.Id,
                    UserName = u.UserName,
                });
            */
        }

        // GET: Blog/Post/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Author)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Blog/Post/Create
        public async Task<IActionResult> Create()
        {
            var categories = await _context.Categories.ToListAsync();
            ViewData["Categories"] = new MultiSelectList(categories, "Id", "Title");

            return View();
        }

        // POST: Blog/Post/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Title,Description,Slug,Content,Published,CategoriesId")]
            CreatePostModel post)
        {
            var categories = await _context.Categories.ToListAsync();
            ViewData["Categories"] = new MultiSelectList(categories, "Id", "Title");

            post.Slug ??= AppUtilities.GenerateSlug(post.Title);

            if (await _context.Posts.AnyAsync(x => x.Slug == post.Slug))
            {
                ModelState.AddModelError("Slug", "Nhập chuỗi Url khác");
                return View(post);
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                post.AuthorId = user.Id;
                post.Author = user;
                post.DateCreated = post.DateUpdated = DateTime.Now;
                _context.Add(post);

                if (post.CategoriesId != null)
                {
                    var checkedCategories =
                        await _context.Categories.Where(x => post.CategoriesId.Contains(x.Id)).ToListAsync();
                    post.Categories = checkedCategories;
                }

                await _context.SaveChangesAsync();
                StatusMessage = "Vừa tạo bài viết mới";
                return RedirectToAction(nameof(Index));
            }

            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .Select(x => new { x.Key, x.Value.Errors })
                .ToArray();

            return View(post);
        }

        // GET: Blog/Post/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.Include(x => x.Categories)
                .FirstOrDefaultAsync(x => x.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            var categoryIds = post.Categories?.Select(x => x.Id).ToArray();

            var postEdit = new CreatePostModel
            {
                PostId = post.PostId,
                Title = post.Title,
                Description = post.Description,
                Slug = post.Slug,
                Content = post.Content,
                Published = post.Published,
                CategoriesId = categoryIds
            };

            var categories = await _context.Categories.ToListAsync();
            ViewData["Categories"] = new MultiSelectList(categories, "Id", "Title");
            return View(postEdit);
        }

        // POST: Blog/Post/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("PostId,Title,Description,Slug,Content,Published,CategoriesId")]
            CreatePostModel post)
        {
            if (id != post.PostId)
            {
                return NotFound();
            }

            post.Slug ??= AppUtilities.GenerateSlug(post.Title);

            if (await _context.Posts.AnyAsync(x => x.Slug == post.Slug && x.PostId != id))
            {
                ModelState.AddModelError("Slug", "Nhập chuỗi Url khác");
                return View(post);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var postUpdate = await _context.Posts.Include(x => x.Categories)
                        .FirstOrDefaultAsync(x => x.PostId == id);
                    if (postUpdate == null)
                    {
                        return NotFound();
                    }

                    postUpdate.Title = post.Title;
                    postUpdate.Description = post.Description;
                    postUpdate.Slug = post.Slug;
                    postUpdate.Published = post.Published;
                    postUpdate.Content = post.Content;
                    postUpdate.DateUpdated = DateTime.Now;

                    /*Update PostCategory Old Version
                    post.CategoriesId ??= Array.Empty<int>();
                    var oldCateIds = postUpdate.Categories?.Select(x => x.Id).ToArray();
                    var newCateIds = post.CategoriesId;

                    var removeCateInPost = postUpdate.Categories?
                        .Where(x => !newCateIds.Contains(x.Id)).ToList();

                    var removeCateInPosts = (from postCate in postUpdate.Categories
                                where !newCateIds.Contains(postCate.Id)
                                select postCate)
                            .ToList()
                        ;
                    _context.CategoryPost.RemoveRange(removeCateInPosts);
                    var addCategories = postUpdate.Categories?.Where(x => oldCateIds != null && !oldCateIds.Contains(x.Id)).ToList();
                    if (addCategories != null)
                        foreach (var category in addCategories)
                        {
                            postUpdate.Categories?.Add(category);
                        }
                    */

                    //Update PostCategory New
                    post.CategoriesId ??= Array.Empty<int>();
                    var addCategories = _context.Categories.Where(x => post.CategoriesId.Contains(x.Id)).ToList();

                    postUpdate.Categories = addCategories;

                    _context.Update(postUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.PostId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                StatusMessage = "Vừa cập nhật bài viết";
                return RedirectToAction(nameof(Index));
            }

            var categories = await _context.Categories.ToListAsync();
            ViewData["Categories"] = new MultiSelectList(categories, "Id", "Title");
            return View(post);
        }

        // GET: Blog/Post/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Author)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Blog/Post/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Posts == null)
            {
                return Problem("Entity set 'MasterDbContext.Posts'  is null.");
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            StatusMessage = "Bạn vừa xóa bài viết : " + post.Title;
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return (_context.Posts?.Any(e => e.PostId == id)).GetValueOrDefault();
        }
    }
}