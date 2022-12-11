using ASP_MVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace ASP_MVC.Areas.ProductManage.Controllers
{
    [Area("ProductManage")]
    public class ProductController : Controller
    {
        private readonly ProductService _productService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(ProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }
        [Route("/cac-san-pham/{id?}")]
        public IActionResult Index()
        {
            // /Areas/AreaName/Views/ControllerName/ActionName.cshtml
            
            var products = _productService.OrderBy(x => x.Name).ToList();
            return View(products); //Areas/ProductManage/Product/Index.cshtml
        }
    }
}