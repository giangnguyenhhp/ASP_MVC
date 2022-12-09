## Controllers
- Là một lớp kế thừa từ lớp Controller : Microsoft.AspnetCore.MVC.Controller
- Action trong Controller là một phương thức public (không được static)
- Action có thể trả về bất kì kiểu dữ lệu nào , thường là IActionResult
- Các dịch vụ Inject vào Controller qua hàm tạo
## View
- Là file .cshtml
- View cho Action lưu tại : Views/ControllerName/ActionName.cshtml
- Thêm thư mục lưu trữ View :
```
  builder.Services.Configure<RazorViewEngineOptions>(option =>
  {
  // Views/Controller/Action.cshtml
  // MyView/Controller/Action.cshtml

  // {0} => tên Action
  // {1} => tên Controller
  // {2} => tên Area
  option.ViewLocationFormats.Add("/MyView/{1}/{0}"+RazorViewEngine.ViewExtension);
  });
```
## Truyền dữ liệu sang View
- Model
- ViewData
- ViewBag
- TempData