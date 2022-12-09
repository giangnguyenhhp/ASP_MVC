using ASP_MVC.Services;
using Microsoft.AspNetCore.Mvc.Razor;

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
    option.ViewLocationFormats.Add("/MyView/{1}/{0}"+RazorViewEngine.ViewExtension);
});
// builder.Services.AddSingleton<ProductService>();
// builder.Services.AddSingleton<ProductService, ProductService>();
// builder.Services.AddSingleton(typeof(ProductService));
builder.Services.AddSingleton(typeof(ProductService), typeof(ProductService));


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

app.UseRouting(); // EndpointRoutingMiddleware

app.UseAuthentication(); // Xác định danh tiính
app.UseAuthorization(); // Xác thực quyền truy cập

//URL : /{controller}/{action}/{id?}
//Abc/Xyz => Controller=Abc gọi tới method Xyz() trong controller Abc
//Home/Index => Controller = Home gọi tới method Index() trong controller Home
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();