using ASP_MVC.Models;
using ASP_MVC.Models.Blog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASP_MVC.Areas.Blog.Controllers
{
    [Area("Blog")]
    public class ViewPostController : Controller
    {
        private readonly ILogger<ViewPostController> _logger;
        private readonly MasterDbContext _context;
        
        public ViewPostController(ILogger<ViewPostController> logger, MasterDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        
        
        // GET: ViewPost
        [HttpGet("/post/{categorySlug?}")]
        public ActionResult Index(string categorySlug,[FromQuery(Name = "p")]int currentPage,int pageSize)
        {
            var categories = GetCategories();
            ViewBag.categories = categories;
            ViewBag.categorySlug = categorySlug;

            Category? category = null;
            if (!string.IsNullOrEmpty(categorySlug))
            {
                category = _context.Categories.Include(x => x.CategoryChildren)
                    .FirstOrDefault(x => x.Slug == categorySlug);
                if (category ==null)
                {
                    return NotFound("Không tìm thấy Category");
                }
            }

            var posts = _context.Posts.OrderByDescending(x => x.DateUpdated)
                .Include(p => p.Author)
                .Include(x => x.Categories)
                .AsQueryable();

            if (category != null)
            {
                var ids = new List<int>();
                category.ChildCategoryIds(ids);
                ids.Add(category.Id);

                posts = posts.Where(p =>p.Categories.Any(x => ids.Contains(x.Id)));
            }
            
            //Phân trang
            var totalPosts =  posts.Count();
            if (pageSize <= 0) pageSize = 5;
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

            var postsInPage =  posts.Skip((currentPage - 1) * pageSize)
                    .Take(pageSize)
                    .ToList()
                ;
            

            if (category != null) ViewBag.category = category;
            return View(postsInPage);
        }

        // GET: ViewPost/Details/5
        [HttpGet("/post/{postSlug}.html")]
        public ActionResult Details(string postSlug)
        {
            var categories = GetCategories();
            ViewBag.categories = categories;

            var post = _context.Posts.Include(p => p.Author)
                .Include(p => p.Categories)
                .FirstOrDefault(p => p.Slug == postSlug);

            if (post == null)
            {
                return NotFound("Không tìm thấy bài viết");
            }

            Category category = post.Categories.FirstOrDefault();
            if (category != null) ViewBag.category = category;

            var otherPosts = _context.Posts
                .Where(p => p.Categories.Any(c => c.Id == category.Id))
                .Where(p => p.PostId != post.PostId)
                .OrderByDescending(p=>p.DateUpdated)
                .Take(5)
                ;
            ViewBag.otherPosts = otherPosts;

            return View(post);
        }

        private List<Category> GetCategories()
        {
            var categories = _context.Categories
                .Include(x=>x.CategoryChildren)
                .AsEnumerable()
                .Where(x=>x.ParentCategory == null)
                .ToList();
            
            return categories;
        }
        
        

        // // GET: ViewPost/Create
        // public ActionResult Create()
        // {
        //     return View();
        // }
        //
        // // POST: ViewPost/Create
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public ActionResult Create(IFormCollection collection)
        // {
        //     try
        //     {
        //         // TODO: Add insert logic here
        //
        //         return RedirectToAction(nameof(Index));
        //     }
        //     catch
        //     {
        //         return View();
        //     }
        // }
        //
        // // GET: ViewPost/Edit/5
        // public ActionResult Edit(int id)
        // {
        //     return View();
        // }
        //
        // // POST: ViewPost/Edit/5
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public ActionResult Edit(int id, IFormCollection collection)
        // {
        //     try
        //     {
        //         // TODO: Add update logic here
        //
        //         return RedirectToAction(nameof(Index));
        //     }
        //     catch
        //     {
        //         return View();
        //     }
        // }
        //
        // // GET: ViewPost/Delete/5
        // public ActionResult Delete(int id)
        // {
        //     return View();
        // }
        //
        // // POST: ViewPost/Delete/5
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public ActionResult Delete(int id, IFormCollection collection)
        // {
        //     try
        //     {
        //         // TODO: Add delete logic here
        //
        //         return RedirectToAction(nameof(Index));
        //     }
        //     catch
        //     {
        //         return View();
        //     }
        // }
    }
}