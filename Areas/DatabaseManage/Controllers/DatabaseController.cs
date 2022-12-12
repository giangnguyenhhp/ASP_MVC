using ASP_MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASP_MVC.Areas.DatabaseManage.Controllers
{
    [Area("DatabaseManage")]
    [Route("database-manage/[action]")]
    public class DatabaseController : Controller
    {
        private readonly MasterDbContext _dbContext;

        public DatabaseController(MasterDbContext dbContext)
        {
            _dbContext = dbContext;
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
    }
}