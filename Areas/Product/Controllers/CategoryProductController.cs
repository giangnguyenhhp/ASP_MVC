using ASP_MVC.Data;
using ASP_MVC.Models;
using ASP_MVC.Models.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ASP_MVC.Areas.Product.Controllers
{
    [Area("Product")]
    [Route("admin/categoryProduct/category/[action]/{id?}")]
    [Authorize(Roles = RoleName.Administrator)]
    public class CategoryProductController : Controller
    {
        private readonly MasterDbContext _context;

        public CategoryProductController(MasterDbContext context)
        {
            _context = context;
        }

        // GET: Blog/Category
        public async Task<IActionResult> Index()
        {
            var qr = _context.CategoryProducts
                .Include(c => c.ParentCategory)
                .ThenInclude(c => c.CategoryChildren);
            var categories = (await qr.ToListAsync()).Where(x => x.ParentCategory == null).ToList();
            return View(categories);
        }

        // GET: Blog/Category/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.CategoryProducts
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        private void CreateSelectItems(List<CategoryProduct> source, List<CategoryProduct> des, int level)
        {
            foreach (var category in source)
            {
                var prefix = string.Concat(Enumerable.Repeat("---", level));
                // category.Title = prefix + " " + category.Title;
                des.Add(new CategoryProduct()
                {
                    Id = category.Id,
                    Title = prefix + " " + category.Title
                });
                if (category.CategoryChildren?.Count > 0)
                {
                    CreateSelectItems(category.CategoryChildren.ToList(), des, level + 1);
                }
            }
        }

        // GET: Blog/Category/Create
        public async Task<IActionResult> Create()
        {
            var qr = _context.CategoryProducts
                .Include(c => c.ParentCategory)
                .ThenInclude(c => c.CategoryChildren);
            var categories = (await qr.ToListAsync()).Where(x => x.ParentCategory == null).ToList();
            categories.Insert(0, new CategoryProduct
            {
                Id = -1,
                Title = "Không có danh mục cha"
            });

            var items = new List<CategoryProduct>();
            CreateSelectItems(categories, items, 0);

            ViewData["ParentCategoryId"] = new SelectList(items, "Id", "Title");
            return View();
        }

        // POST: Blog/Category/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(
            [Bind("Title,Description,Slug,ParentCategoryId")]
            CategoryProduct category)
        {
            if (ModelState.IsValid)
            {
                if (category.ParentCategoryId == -1) category.ParentCategoryId = null;
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .Select(x => new { x.Key, x.Value.Errors })
                .ToArray();

            var qr = _context.CategoryProducts
                .Include(c => c.ParentCategory)
                .ThenInclude(c => c.CategoryChildren);
            var categories = (await qr.ToListAsync()).Where(x => x.ParentCategory == null).ToList();
            categories.Insert(0, new CategoryProduct
            {
                Id = -1,
                Title = "Không có danh mục cha"
            });

            var items = new List<CategoryProduct>();
            CreateSelectItems(categories, items, 0);

            ViewData["ParentCategoryId"] = new SelectList(items, "Id", "Title", category.ParentCategoryId);
            return View(category);
        }

        // GET: Blog/Category/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.CategoryProducts == null)
            {
                return NotFound();
            }

            var category = await _context.CategoryProducts.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var qr = _context.CategoryProducts
                .Include(c => c.ParentCategory)
                .ThenInclude(c => c.CategoryChildren);
            var categories = (await qr.ToListAsync()).Where(x => x.ParentCategory == null).ToList();
            categories.Insert(0, new CategoryProduct
            {
                Id = -1,
                Title = "Không có danh mục cha"
            });

            var items = new List<CategoryProduct>();
            CreateSelectItems(categories, items, 0);

            ViewData["ParentCategoryId"] = new SelectList(items, "Id", "Title", category.ParentCategoryId);
            return View(category);
        }

        // POST: Blog/Category/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,Title,Description,Slug,ParentCategoryId")]
            CategoryProduct category)
        {
            var canUpdate = true;
            if (id != category.Id)
            {
                return NotFound();
            }

            if (category.ParentCategoryId == category.Id)
            {
                ModelState.AddModelError(string.Empty, "Phải chọn danh mục cha khác");
                canUpdate = false;
            }

            //Kiểm tra thiết lập danh mục cha phù hợp
            if (canUpdate && category.ParentCategoryId != null)
            {
                var childCategory = await _context.CategoryProducts
                    .Include(x => x.CategoryChildren)
                    .Where(x => x.ParentCategoryId == category.Id).ToListAsync();

                //Func check Id
                Func<List<CategoryProduct>, bool> checkCateIds = null;
                checkCateIds = (cates) =>
                {
                    foreach (var cate in cates)
                    {
                        Console.WriteLine(cate.Title);
                        if (cate.Id == category.ParentCategoryId)
                        {
                            canUpdate = false;
                            ModelState.AddModelError(string.Empty, "Phải chọn danh mục cha khác");
                            return true;
                        }

                        if (cate.CategoryChildren != null)
                        {
                            return checkCateIds(cate.CategoryChildren.ToList());
                        }
                    }

                    return false;
                };
                //End Func
                checkCateIds(childCategory.ToList());
            }

            if (ModelState.IsValid && canUpdate)
            {
                try
                {
                    if (category.ParentCategoryId == -1)
                    {
                        category.ParentCategoryId = null;
                    }

                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryProductExists(category.Id))
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

            var qr = _context.CategoryProducts
                .Include(c => c.ParentCategory)
                .ThenInclude(c => c.CategoryChildren);
            var categories = (await qr.ToListAsync()).Where(x => x.ParentCategory == null).ToList();
            categories.Insert(0, new CategoryProduct()
            {
                Id = -1,
                Title = "Không có danh mục cha"
            });

            var items = new List<CategoryProduct>();
            CreateSelectItems(categories, items, 0);

            ViewData["ParentCategoryId"] = new SelectList(items, "Id", "Title", category.ParentCategoryId);
            return View(category);
        }

        // GET: Blog/Category/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.CategoryProducts == null)
            {
                return NotFound();
            }

            var category = await _context.CategoryProducts
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Blog/Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Categories == null)
            {
                return Problem("Entity set 'MasterDbContext.Categories'  is null.");
            }

            var category = await _context.CategoryProducts
                .Include(c => c.CategoryChildren)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            if (category.CategoryChildren != null)
                foreach (var categoryChild in category.CategoryChildren)
                {
                    categoryChild.ParentCategoryId = category.ParentCategoryId;
                }

            _context.CategoryProducts.Remove(category);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryProductExists(int id)
        {
            return (_context.CategoryProducts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}