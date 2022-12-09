using ASP_MVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace ASP_MVC.Controllers;

public class FirstController : Controller
{
    private readonly ILogger<FirstController> _logger;
    private readonly IWebHostEnvironment _environment;
    private readonly ProductService _productService;
    public FirstController(ILogger<FirstController> logger, IWebHostEnvironment environment, ProductService service)
    {
        _logger = logger;
        _environment = environment;
        _productService = service;
    }

    public string Index()
    {
        // HttpContext
        // Request
        // Response
        // RouteData

        // User
        // ModelState
        // ViewData
        // ViewBag
        // Url
        // TempData

        _logger.LogTrace("Thông báo");
        _logger.LogCritical("Thông báo");
        _logger.LogError("Thông báo");
        _logger.LogDebug("Thông báo");
        _logger.LogWarning("Thông báo");
        _logger.LogInformation("Index Action");
        // Console.WriteLine("Index Action");
        return "Tôi là Index của First";
    }

    public void Nothing()
    {
        // EmptyResult                 | new EmptyResult() : trả về empty
        _logger.LogInformation("Nothing Action");
        Response.Headers.Add("hi", "xin chao cac ban");
    }

    public object Anything() => new[] { 1, 2, 3 };

    public IActionResult Readme()
    {
        // ContentResult               | Content(string content,string contentType) : trả về một content với tham số là content muốn hiển thị
        var content = @"
            Xin chào các bạn,
            các bạn đang học về ASP.NET MVC





            XUANTHULAB.NET            
            ";
        return Content(content,"text/html");
    }

    public IActionResult Image()
    {   
        // FileResult                  | File(byte[] fileContent,string contentType) : trả về một file với tham số là một mảng byte của ảnh đó được đọc
        // Path.Combine : nối các param thành một đường dẫn hoàn chỉnh
        var filePath = Path.Combine(_environment.ContentRootPath, "Image", "picture1.jpg");
        var bytes = System.IO.File.ReadAllBytes(filePath);
        return File(bytes, "image/jpg");
    }

    public IActionResult IphonePrice()
    {
        // JsonResult                  | Json(object data) : tạo một đối tương có kiểu JsonResult rồi chuyển đối tượng thành kiểu dữ liệu Json
        return Json(
            new
            {
                productName = "Iphone 11",
                Price = 1000
            });
    }

    public IActionResult Privacy()
    {
        // LocalRedirectResult         | LocalRedirect(string localUrl) : chuyển về một url nội bộ localhost:5001
        var url = Url.Action("Privacy", "Home");// => url = localhost:5001/Home/Privacy
        _logger.LogInformation("Chuyen huong den : " + url);
        return LocalRedirect(url);
    }

    public IActionResult Google()
    {
        // RedirectResult              | Redirect(string url) : trả về một url bất kì
        var url = "https://google.com.vn";
        _logger.LogInformation("Chuyen huong den : " + url);
        return Redirect(url);
    }
    
    // IActionResult
    // ViewResult                  | View()
    public IActionResult HelloView(string username)
    {
        if (string.IsNullOrEmpty(username))
        {
            username = "Khách";
        }
        //View() => Razor Engine , đọc .cshtml (template)
        //-----------------------------------------------
        //View(template) => template : đường dẫn tuyệt đối tới file .cshtml
        //View(template, model) => truyền model cho View hiển thị
        // return View("/MyView/xinchao1.cshtml",username);

        //xinchao2.cshtml => Views/First/xinchao2.cshtml || Views/Shared/xinchao2.cshtml || Pages/Shared/xinchao2.cshtml
        // return View("xinchao2", username);

        //HelloView.cshtml => Views/First/HelloView.cshtml || Views/Shared/HelloView.cshtml || Pages/Shared/HelloView.cshtml
        //Views/Controller/Action.cshtml => Views/First/HelloView.cshtml
        return View((object)username);
        
        // Add thêm MyView vào Razor View Engine => MyView/Controller/Action.cshtml => MyView/First/xinchao3.cshtml
        // return View("xinchao3",username);
        
        // View();
        // View(Models);
    }
    [TempData]
    public string StatusMessage { get; set; }

    public IActionResult ViewProduct(int id)
    {
        var product = _productService.FirstOrDefault(x=>x.Id == id);
        if (product == null)
        {
            // TempData["StatusMessage"] = "Sản phẩm không tồn tại";
            StatusMessage = "Sản phẩm không tồn tại";
            return LocalRedirect(Url.Action("Index", "Home") ?? string.Empty);
        }
        
        //Views/First/ViewProduct.cshtml
        //MyView/First/ViewProduct.cshtml
        
        //Model
        // return View(product);
        
        //ViewData
        // ViewData["product"]=product;
        // ViewData["Title"] = product.Name;
        // return View("ViewProduct2");
        
        // ViewBag
        ViewBag.abc = "View Product";
        ViewBag.product = product;
        return View("ViewProduct3");
        
    }

    
    
    
    // ForbidResult                | Forbid()
    // RedirectToActionResult      | RedirectToAction()
    // RedirectToPageResult        | RedirectToRoute()
    // RedirectToRouteResult       | RedirectToPage()
    // PartialViewResult           | PartialView()
    // ViewComponentResult         | ViewComponent()
    // StatusCodeResult            | StatusCode()
}