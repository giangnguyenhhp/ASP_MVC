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

## Areas
- Là tên dùng để routing
- Là cấu trúc thư mục chứa M.V.C
- Thiết lập Areas cho Controller bằng ```[Areas("NameArea")]```
- Tạo cấu trúc thư mục 
```
dotnet aspnet-codegenerator area NameArea
```
## Route
- app.MapControllerRoute()
- ap.MapAreaControllerRoute()
- [AcceptVerbs("POST","GET")]
- [Route("pattern")]
- [HttpGet]
- [HttpPost]
## Url generation
### UrlHelper : Action,ActionLink,RouteUrl,link
```js
Url.Action("PlanetInfo","Planet",new{id=1},Context.Request.Scheme)

Url.RouteUrl("default",new{controller="First",action="HelloView",id=1,username="GCP"})
```
### HtmlTagHelper : ```<a> <button> <form>```
Sử dụng thuộc tính :
```
asp-area="AreaName"
asp-controller="controllerName"
asp-action="actionName"
asp-route="RouteName"
asp-route-...="xxx"
asp-route-id="1"
asp-route-name="gcp"
```