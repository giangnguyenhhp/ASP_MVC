using ASP_MVC.Data;
using ASP_MVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASP_MVC.Areas.DatabaseManage.Controllers
{
    [Area("DatabaseManage")]
    [Route("database-manage/[action]")]
    public class DatabaseController : Controller
    {
        private readonly MasterDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DatabaseController(MasterDbContext dbContext, RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult DeleteDb()
        {
            return View();
        }

        [TempData] public string StatusMessage { get; set; }

        [HttpPost]
        public async Task<IActionResult> DeleteDbAsync()
        {
            var success = await _dbContext.Database.EnsureDeletedAsync();
            StatusMessage = success ? "Xóa Database thành công" : "Không thể xóa Database";
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> CreateDb()
        {
            await _dbContext.Database.MigrateAsync();
            StatusMessage = "Cập nhật Database thành công";
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> RegisterAdminAsync()
        {
            //Create Roles
            var roleNames = RoleName.DefaultRoles;
            foreach (var roleName in roleNames)
            {
                var roleCheck = await _roleManager.FindByNameAsync(roleName);
                if (roleCheck==null)
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
            
            //Create User admin, pass=admin123 , admin@example.com
            var userAdmin = await _userManager.FindByNameAsync("admin");
            if (userAdmin==null)
            {
                userAdmin = new AppUser()
                {
                    UserName = "admin",
                    Email = "admin@example.com",
                    EmailConfirmed = true,
                };
                var result = await _userManager.CreateAsync(userAdmin, "admin123");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(userAdmin, RoleName.Administrator);
                }
            }

            StatusMessage = " Vừa seed Database thành công ";
            return RedirectToAction("Index");
        }
    }
}