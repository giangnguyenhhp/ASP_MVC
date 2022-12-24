using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ASP_MVC.Areas.Product.Models;
using ASP_MVC.Data;
using ASP_MVC.Models;
using ASP_MVC.Models.Blog;
using ASP_MVC.Models.Product;
using ASP_MVC.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ASP_MVC.Areas.Product.Controllers
{
    [Area("Product")]
    [Route("admin/productManage/[action]/{id?}")]
    [Authorize(Roles = RoleName.Administrator)]
    public class ProductManageController : Controller
    {
        private readonly MasterDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ProductManageController(MasterDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [TempData] public string StatusMessage { get; set; }

        // GET: Blog/Post
        public async Task<IActionResult> Index([FromQuery(Name = "p")] int currentPage, int pageSize)
        {
            var products = _context.Products
                .Include(p => p.Author)
                .OrderByDescending(x => x.DateCreated);

            var totalPosts = await products.CountAsync();
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

            var productInPage = await products.Skip((currentPage - 1) * pageSize)
                    .Take(pageSize)
                    .Include(p => p.CategoryProducts)
                    .ToListAsync()
                ;

            return View(productInPage);

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
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Author)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Blog/Post/Create
        public async Task<IActionResult> Create()
        {
            var categoryProducts = await _context.CategoryProducts.ToListAsync();
            ViewData["Categories"] = new MultiSelectList(categoryProducts, "Id", "Title");

            return View();
        }

        // POST: Blog/Post/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Title,Description,Slug,Content,Published,CategoryProductIds,Price")]
            CreateProductModel product)
        {
            var categoryProducts = await _context.CategoryProducts.ToListAsync();
            ViewData["Categories"] = new MultiSelectList(categoryProducts, "Id", "Title");

            product.Slug ??= AppUtilities.GenerateSlug(product.Title);

            if (await _context.Products.AnyAsync(x => x.Slug == product.Slug))
            {
                ModelState.AddModelError("Slug", "Nhập chuỗi Url khác");
                return View(product);
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                product.AuthorId = user.Id;
                product.Author = user;
                product.DateCreated = product.DateUpdated = DateTime.Now;
                _context.Add(product);

                if (product.CategoryProductIds != null)
                {
                    var checkedCategoryProducts =
                        await _context.CategoryProducts.Where(x => product.CategoryProductIds.Contains(x.Id))
                            .ToListAsync();
                    product.CategoryProducts = checkedCategoryProducts;
                }

                await _context.SaveChangesAsync();
                StatusMessage = "Vừa tạo sản phẩm mới";
                return RedirectToAction(nameof(Index));
            }

            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .Select(x => new { x.Key, x.Value.Errors })
                .ToArray();

            return View(product);
        }

        // GET: Blog/Post/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(x => x.CategoryProducts)
                .FirstOrDefaultAsync(x => x.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            var categoryProductIds = product.CategoryProducts?.Select(x => x.Id).ToArray();

            var productEdit = new CreateProductModel()
            {
                ProductId = product.ProductId,
                Title = product.Title,
                Description = product.Description,
                Slug = product.Slug,
                Content = product.Content,
                Published = product.Published,
                CategoryProductIds = categoryProductIds,
                Price = product.Price
            };

            var categoryProducts = await _context.CategoryProducts.ToListAsync();
            ViewData["Categories"] = new MultiSelectList(categoryProducts, "Id", "Title");
            return View(productEdit);
        }

        // POST: Blog/Post/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("ProductId,Title,Description,Slug,Content,Published,CategoryProductIds,Price")]
            CreateProductModel product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            product.Slug ??= AppUtilities.GenerateSlug(product.Title);

            if (await _context.Products.AnyAsync(x => x.Slug == product.Slug && x.ProductId != id))
            {
                ModelState.AddModelError("Slug", "Nhập chuỗi Url khác");
                return View(product);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var productUpdate = await _context.Products
                        .Include(x => x.CategoryProducts)
                        .FirstOrDefaultAsync(x => x.ProductId == id);
                    if (productUpdate == null)
                    {
                        return NotFound();
                    }

                    productUpdate.Title = product.Title;
                    productUpdate.Description = product.Description;
                    productUpdate.Slug = product.Slug;
                    productUpdate.Published = product.Published;
                    productUpdate.Content = product.Content;
                    productUpdate.DateUpdated = DateTime.Now;
                    productUpdate.Price = product.Price;

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

                    //Update ProductCategory New
                    product.CategoryProductIds ??= Array.Empty<int>();
                    var addCategoryProducts = _context.CategoryProducts
                        .Where(x => product.CategoryProductIds.Contains(x.Id)).ToList();

                    productUpdate.CategoryProducts = addCategoryProducts;

                    _context.Update(productUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                StatusMessage = "Vừa cập nhật sản phẩm";
                return RedirectToAction(nameof(Index));
            }

            var categoryProducts = await _context.CategoryProducts.ToListAsync();
            ViewData["Categories"] = new MultiSelectList(categoryProducts, "Id", "Title");
            return View(product);
        }

        // GET: Blog/Post/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Author)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Blog/Post/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'MasterDbContext.Products'  is null.");
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            StatusMessage = "Bạn vừa xóa sản phẩm : " + product.Title;
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return (_context.Products?.Any(e => e.ProductId == id)).GetValueOrDefault();
        }


        public class UploadOneFile
        {
            [Required(ErrorMessage = "Phải chọn file upload")]
            [DataType(DataType.Upload)]
            [FileExtensions(Extensions = "png,jpeg,jpg,gif")]
            [DisplayName("Chọn file upload")]
            public IFormFile FileUpload { get; set; }
        }

        [HttpGet]
        public IActionResult UploadPhoto(int id)
        {
            var product = _context.Products
                .Include(p => p.ProductPhotos)
                .FirstOrDefault(x => x.ProductId == id);

            if (product == null)
            {
                return NotFound("Không có sản phẩm");
            }

            ViewData["Product"] = product;

            return View(new UploadOneFile());
        }

        [HttpPost, ActionName("UploadPhoto")]
        public async Task<IActionResult> UploadPhotoAsync(int id, [Bind("FileUpload")] UploadOneFile? file)
        {
            var product = await _context.Products
                .Include(p => p.ProductPhotos)
                .FirstOrDefaultAsync(x => x.ProductId == id);

            if (product == null)
            {
                return NotFound("Không có sản phẩm");
            }

            ViewData["Product"] = product;

            if (file != null)
            {
                var fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName())
                            + Path.GetExtension(file.FileUpload.FileName);

                var filePath = Path.Combine("Uploads", "Products", fileName);
                await using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.FileUpload.CopyToAsync(fileStream);
                }


                _context.Add(new ProductPhoto()
                {
                    Product = product,
                    FileName = fileName
                });

                await _context.SaveChangesAsync();
            }

            return View(new UploadOneFile());
        }

        [HttpPost]
        public async Task<IActionResult> ListPhotos(int id)
        {
            var product = await _context.Products
                .Include(p => p.ProductPhotos)
                .FirstOrDefaultAsync(x => x.ProductId == id);

            if (product == null)
            {
                return Json(
                    new
                    {
                        success = 0,
                        message = "Product not found"
                    });
            }

            var listPhotos = product.ProductPhotos.Select(photo => new
            {
                id = photo.Id,
                path = "/Uploads/Products/" + photo.FileName
            });

            return Json(new
            {
                success = 1,
                photos = listPhotos
            });
        }

        [HttpPost]
        public IActionResult DeletePhoto(int id)
        {
            var photo = _context.ProductPhotos.FirstOrDefault(x => x.Id == id);
            if (photo != null)
            {
                _context.ProductPhotos.Remove(photo);
                _context.SaveChanges();

                var fileName = "Uploads/Products/" + photo.FileName;
                System.IO.File.Delete(fileName);
            }

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> UploadPhotoApi(int id, [Bind("FileUpload")] UploadOneFile? file)
        {
            var product = await _context.Products
                .Include(p => p.ProductPhotos)
                .FirstOrDefaultAsync(x => x.ProductId == id);

            if (product == null)
            {
                return NotFound("Không có sản phẩm");
            }

            if (file != null)
            {
                var fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName())
                            + Path.GetExtension(file.FileUpload.FileName);

                var filePath = Path.Combine("Uploads", "Products", fileName);
                await using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.FileUpload.CopyToAsync(fileStream);
                }


                _context.Add(new ProductPhoto()
                {
                    Product = product,
                    FileName = fileName
                });

                await _context.SaveChangesAsync();
            }

            return Ok();
        }
    }
}