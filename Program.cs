using ASP_MVC.ExtendMethods;
using ASP_MVC.Models;
using ASP_MVC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;

// ReSharper disable All

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// Add service Razor Pages
builder.Services.AddRazorPages();
//Add option cho Razor View Engine
builder.Services.Configure<RazorViewEngineOptions>(option =>
{
    // Views/Controller/Action.cshtml
    // MyView/Controller/Action.cshtml

    // {0} => tên Action
    // {1} => tên Controller
    // {2} => tên Area
    option.ViewLocationFormats.Add("/MyView/{1}/{0}" + RazorViewEngine.ViewExtension);
});
// builder.Services.AddSingleton<ProductService>();
// builder.Services.AddSingleton<ProductService, ProductService>();
// builder.Services.AddSingleton(typeof(ProductService));
builder.Services.AddSingleton(typeof(ProductService), typeof(ProductService));
builder.Services.AddSingleton<PlanetService>();

//Connect to PostgreSQl
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

//For Entity Framework
builder.Services.AddDbContext<MasterDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.AddStatusCodePage(); // Tùy biến Response có lỗi từ 400 - 599

app.UseRouting(); // EndpointRoutingMiddleware

app.UseAuthentication(); // Xác định danh tính
app.UseAuthorization(); // Xác thực quyền truy cập

//URL : /{controller}/{action}/{id?}
//Abc/Xyz => Controller=Abc gọi tới method Xyz() trong controller Abc
//Home/Index => Controller = Home gọi tới method Index() trong controller Home
// app.MapControllerRoute(
//     name: "default",
//     pattern: "{controller=Home}/{action=Index}/{id?}");
//
// app.MapRazorPages();

//EndPoint : sayHi
app.MapGet("/sayHi", async context =>
{
    await context.Response.WriteAsync($"Hello ASP.MVC - {DateTime.Now}");
});

// app.MapControllers();
// app.MapControllerRoute();
// app.MapDefaultControllerRoute();
// app.MapAreaControllerRoute();

// [AcceptVerbs()] : cho phép truy vấn action bằng method Get,Post,Put,...

// [Route("")]

// [HttpGet]
// [HttpPost]
// [HttpPut]
// [HttpDelete]
// [HttpPatch]
// [HttpHead]

app.MapControllers();

// xemsanpham/1
// adadad/1
// Home/1
app.MapControllerRoute(
    name: "first",
    pattern:"{url:regex(^((xemsanpham))|((viewproduct))$)}/{id:range(2,4)}",
    // Các giá trị của controller và action,id trong pattern
    defaults: new
    {
        controller = "First",
        action = "ViewProduct",
        // id=1
    },
    // Các điều kiện ràng  buộc của pattern có thể viết trực tiếp trong pattern bằng kí tự ":" sau các thành phần trong pattern
    constraints: new
    {
        // IRouteConstraint
        // url = new RegexRouteConstraint(@"^((xemsanpham))|((viewproduct))"),
        // id = new RangeRouteConstraint(2,4)
    }
    );

//Area
app.MapAreaControllerRoute(
    name:"product",
    pattern:"{controller}/{action=Index}/{id?}",
    areaName:"ProductManage"
    );

// Controller không có Area
app.MapControllerRoute(
    name:"default",
    pattern:"{controller=Home}/{action=Index}/{id?}"
    // defaults: new
    // {
    //     controller = "First",
    //     action = "ViewProduct",
    //     Id = 1
    // }
    );

app.MapRazorPages();

app.Run();