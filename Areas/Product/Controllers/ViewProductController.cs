using ASP_MVC.Areas.Product.Models;
using ASP_MVC.Areas.Product.Service;
using ASP_MVC.Models;
using ASP_MVC.Models.Product;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASP_MVC.Areas.Product.Controllers
{
    [Area("Product")]
    public class ViewProductController : Controller
    {
        private readonly ILogger<ViewProductController> _logger;
        private readonly MasterDbContext _context;
        private readonly CartService _cartService;

        public ViewProductController(ILogger<ViewProductController> logger, MasterDbContext context,
            CartService cartService)
        {
            _logger = logger;
            _context = context;
            _cartService = cartService;
        }


        // GET: ViewProduct/categorySlug
        [HttpGet("/product/{categorySlug?}")]
        public ActionResult Index(string categorySlug, [FromQuery(Name = "p")] int currentPage, int pageSize)
        {
            var categories = GetCategories();
            ViewBag.categories = categories;
            ViewBag.categorySlug = categorySlug;

            CategoryProduct? category = null;
            if (!string.IsNullOrEmpty(categorySlug))
            {
                category = _context.CategoryProducts
                    .Include(x => x.CategoryChildren)
                    .FirstOrDefault(x => x.Slug == categorySlug);
                if (category == null)
                {
                    return NotFound("Không tìm thấy Category");
                }
            }

            var products = _context.Products
                .OrderByDescending(x => x.DateUpdated)
                .Include(p => p.Author)
                .Include(p => p.ProductPhotos)
                .Include(x => x.CategoryProducts)
                .AsQueryable();

            if (category != null)
            {
                var ids = new List<int>();
                category.ChildCategoryIds(ids);
                ids.Add(category.Id);

                products = products.Where(p => p.CategoryProducts.Any(x => ids.Contains(x.Id)));
            }

            //Phân trang
            var totalProducts = products.Count();
            if (pageSize <= 0) pageSize = 6;
            var countPages = (int)Math.Ceiling((double)totalProducts / pageSize);

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
            ViewBag.totalPosts = totalProducts;

            ViewBag.postIndex = (currentPage - 1) * pageSize;

            var productsInPage = products.Skip((currentPage - 1) * pageSize)
                    .Take(pageSize)
                    .ToList()
                ;


            if (category != null) ViewBag.category = category;
            return View(productsInPage);
        }

        // GET: ViewProduct/productSlug.html
        [HttpGet("/product/{productSlug}.html")]
        public ActionResult Details(string productSlug)
        {
            var categories = GetCategories();
            ViewBag.categories = categories;

            var product = _context.Products.Include(p => p.Author)
                .Include(p => p.ProductPhotos)
                .Include(p => p.CategoryProducts)
                .FirstOrDefault(p => p.Slug == productSlug);

            if (product == null)
            {
                return NotFound("Không tìm thấy bài viết");
            }

            var category = product?.CategoryProducts?.FirstOrDefault();
            if (category != null) ViewBag.category = category;

            var otherProducts = _context.Products
                    .Where(p => p.CategoryProducts.Any(c => c.Id == category.Id))
                    .Where(p => p.ProductId != product.ProductId)
                    .OrderByDescending(p => p.DateUpdated)
                    .Take(5)
                ;
            ViewBag.otherProducts = otherProducts;

            return View(product);
        }

        private List<CategoryProduct> GetCategories()
        {
            var categories = _context.CategoryProducts
                .Include(x => x.CategoryChildren)
                .AsEnumerable()
                .Where(x => x.ParentCategory == null)
                .ToList();

            return categories;
        }

        //Thêm sản phẩm vào Cart
        [Route("addCart/{productId:int}", Name = "addCart")]
        public IActionResult AddToCart([FromRoute] int productId)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == productId);
            if (product == null)
            {
                return NotFound("Sản phẩm không tồn tại");
            }

            //Xử lý đưa vào Cart
            var cart = _cartService.GetCartItems();
            var cartItem = cart?.Find(p => p.ProductModel.ProductId == productId);
            if (cartItem != null)
            {
                //Đã tồn tại, tăng thêm 1
                cartItem.quantity++;
            }
            else
            {
                cart?.Add(new CartItem()
                {
                    ProductModel = product,
                    quantity = 1
                });
            }

            //Lưu Cart vào session
            if (cart != null) _cartService.SaveCartSession(cart);
            //Chuyển đến trang hiển thị Cart
            return RedirectToAction(nameof(Cart));
        }

        // Hiển thị giỏ hàng
        [Route("/cart", Name = "cart")]
        public IActionResult Cart()
        {
            return View(_cartService.GetCartItems());
        }

        /// xóa item trong cart
        [Route("/removecart/{productId:int}", Name = "removecart")]
        public IActionResult RemoveCart([FromRoute] int productId)
        {
            var cart = _cartService.GetCartItems();
            var cartItem = cart.Find(p => p.ProductModel.ProductId == productId);
            if (cartItem != null)
            {
                // Đã tồn tại, tăng thêm 1
                cart.Remove(cartItem);
            }

            _cartService.SaveCartSession(cart);
            return RedirectToAction(nameof(Cart));
        }

        /// Cập nhật
        [Route("/updatecart", Name = "updatecart")]
        [HttpPost]
        public IActionResult UpdateCart([FromForm] int productId, [FromForm] int quantity)
        {
            // Cập nhật Cart thay đổi số lượng quantity ...
            var cart = _cartService.GetCartItems();
            var cartItem = cart?.Find(p => p.ProductModel.ProductId == productId);
            if (cartItem != null)
            {
                // Đã tồn tại, tăng thêm 1
                cartItem.quantity = quantity;
            }

            _cartService.SaveCartSession(cart);
            // Trả về mã thành công (không có nội dung gì - chỉ để Ajax gọi)
            return Ok();
        }

        [Route("checkout")]
        public IActionResult Checkout()
        {
            var cart = _cartService.GetCartItems();
            //... Xử lý đơn hàng
            
            _cartService.ClearCart();

            return Content("Đã gửi đơn hàng");
        }
    }
}