using ASP_MVC.Data;
using ASP_MVC.ExtendMethods;
using ASP_MVC.Models;
using ASP_MVC.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

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
builder.Services.AddSingleton<IdentityErrorDescriber, AppIdentityErrorDescriber>();

//Add SendEmail Service
builder.Services.AddOptions();
var mailSettings = builder.Configuration.GetSection("MailSettings");
builder.Services.Configure<MailSettings>(mailSettings);
builder.Services.AddSingleton<IEmailSender, SendMailService>();

//Connect to PostgreSQl
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

//For Entity Framework
builder.Services.AddDbContext<MasterDbContext>(options =>
    options.UseNpgsql(connectionString));

//Đăng kí Identity Framework cho ứng dụng
builder.Services.AddIdentity<AppUser, IdentityRole>(options => // IdentityOptions
    {
        //Thiết lập về password 
        options.Password.RequireDigit = false; //Không bắt phải có số
        options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
        options.Password.RequireUppercase = false; //Không bắt phải có chữ hoa
        options.Password.RequireNonAlphanumeric = false; //Không bắt phải có kí tự đặc biệt
        options.Password.RequiredLength = 3; // Độ dài tối thiểu 3 kí tự
        options.Password.RequiredUniqueChars = 1; // Số kí tự riêng biệt

        // Cấu hình lockout - khóa user
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2); // Khóa 2 phút
        options.Lockout.MaxFailedAccessAttempts = 3; // Thất bại 3 lần là khóa
        options.Lockout.AllowedForNewUsers = true;

        // Cấu hình về user
        options.User.AllowedUserNameCharacters = // các kí tự đặt tên user
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
        options.User.RequireUniqueEmail = true; // Email là duy nhất

        // Cấu hình đăng nhập
        options.SignIn.RequireConfirmedEmail = true; // Yêu cầu confrim Email
        options.SignIn.RequireConfirmedPhoneNumber = false; //Yêu cầu confrim sđt
        options.SignIn.RequireConfirmedAccount = true; // Yêu cầu xác minh tài khoản
    })
    .AddEntityFrameworkStores<MasterDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login/";
    options.LogoutPath = "/logout/";
    options.AccessDeniedPath = "/khongduoctruycap.html";
});

//Add Service Authorization Policy
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AllowEditRole", policyBuilder => //Admin/Role/Index
    {
        //Điều kiện của Policy
        policyBuilder.RequireAuthenticatedUser();
        policyBuilder.RequireRole("Admin");
        policyBuilder.RequireRole("Editor");
        // policyBuilder.RequireClaim("Bằng Lái Xe", "Bằng B1");

        //Claims-based authorization
        // policyBuilder.RequireClaim("ClaimType", "ClaimValue1", "giatri2");
        // policyBuilder.RequireClaim("ClaimType", new string[]
        // {
        //     "giatri1", "giatri2"
        // });
        // IdentityRoleClaim<string> claim1;
        // IdentityUserClaim<string> claim2;
        // Claim claim3;
    });
    options.AddPolicy("ShowAdminMenu", pb =>
    {
        pb.RequireAuthenticatedUser();
        pb.RequireRole(RoleName.Administrator);
    });
});

//Add Service Authentication Google,Facebook,Twitte
builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        var gConfig = builder.Configuration.GetSection("Authentication:Google");
        options.ClientId = gConfig["ClientId"] ?? string.Empty;
        options.ClientSecret = gConfig["ClientSecret"] ?? string.Empty;
        //https://localhost:5001/sigin-google
        options.CallbackPath = "/dang-nhap-tu-google";
    })
    .AddFacebook(options =>
    {
        var fConfig = builder.Configuration.GetSection("Authentication:Facebook");
        options.AppId = fConfig["AppId"] ?? string.Empty;
        options.AppSecret = fConfig["AppSecret"] ?? string.Empty;
        //https://localhost:5001/sigin-facebook
        options.CallbackPath = "/dang-nhap-tu-facebook";
    })
    // .AddTwitter()
    // .AddMicrosoftAccount()
    ;

//Đăng kí Identity UI : giao diện mặc định hệ thống tự sinh ra
// builder.Services.AddDefaultIdentity<User>()
//     .AddEntityFrameworkStores<MasterDbContext>()
//     .AddDefaultTokenProviders();

var app = builder.Build();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Sử dụng file tĩnh trong thư mục gốc wwwroot

app.UseStaticFiles(new StaticFileOptions() // Tùy biến sử dụng file tĩnh trong một thư mục khác wwwroot ví dụ Uploads
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(),"Uploads")),
    RequestPath = "/Uploads",
});

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
app.MapGet("/sayHi", async context => { await context.Response.WriteAsync($"Hello ASP.MVC - {DateTime.Now}"); });

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
    pattern: "{url:regex(^((xemsanpham))|((viewproduct))$)}/{id:range(2,4)}",
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
app.MapControllerRoute(
    name: "MyArea",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );

// Controller không có Area
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
    // defaults: new
    // {
    //     controller = "First",
    //     action = "ViewProduct",
    //     Id = 1
    // }
);

app.MapRazorPages();

app.Run();