using ASP_MVC.Data;
using ASP_MVC.Models;
using ASP_MVC.Models.Blog;
using ASP_MVC.Models.Product;
using Bogus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASP_MVC.Areas.DatabaseManage.Controllers
{
    [Area("DatabaseManage")]
    [Route("database-manage/[action]")]
    [Authorize(Roles = RoleName.Administrator)]
    public class DatabaseController : Controller
    {
        private readonly MasterDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DatabaseController(MasterDbContext dbContext, RoleManager<IdentityRole> roleManager,
            UserManager<AppUser> userManager)
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

        public async Task<IActionResult> SeedDataAsync()
        {
            //Create Roles
            var roleNames = RoleName.DefaultRoles;
            foreach (var roleName in roleNames)
            {
                var roleCheck = await _roleManager.FindByNameAsync(roleName);
                if (roleCheck == null)
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            //Create User admin, pass=admin123 , admin@example.com
            var userAdmin = await _userManager.FindByNameAsync("admin");
            if (userAdmin == null)
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

            SeedPostCategory();
            SeedProductCategory();

            StatusMessage = " Vừa seed Database thành công ";
            return RedirectToAction("Index");
        }

        private void SeedProductCategory()
        {
            //Faker Category
            _dbContext.CategoryProducts.RemoveRange(_dbContext.CategoryProducts
                .Where(x => x.Description.Contains("[FakeData]")));
            _dbContext.Products
                .RemoveRange(_dbContext.Products
                    .Include(p => p.ProductPhotos)
                    .Where(x => x.Content.Contains("[FakeData]")));
            _dbContext.SaveChanges();

            var fakerCategory = new Faker<CategoryProduct>();
            var cm = 1;
            fakerCategory.RuleFor(c => c.Title,
                fk => $"Nhóm SP {cm++} " + fk.Lorem.Sentence(1, 2).Trim('.'));
            fakerCategory.RuleFor(c => c.Description,
                fk => fk.Lorem.Sentences(5) + "[FakeData]");
            fakerCategory.RuleFor(c => c.Slug,
                fk => fk.Lorem.Slug());

            var cate1 = fakerCategory.Generate();
            var cate11 = fakerCategory.Generate();
            var cate12 = fakerCategory.Generate();
            var cate2 = fakerCategory.Generate();
            var cate21 = fakerCategory.Generate();
            var cate211 = fakerCategory.Generate();

            cate11.ParentCategory = cate1;
            cate12.ParentCategory = cate1;
            cate21.ParentCategory = cate2;
            cate211.ParentCategory = cate21;

            var categoryProducts = new[] { cate1, cate11, cate2, cate12, cate21, cate211 };
            _dbContext.CategoryProducts.AddRange(categoryProducts);

            //Faker Product
            var rCateIndex = new Random();
            var bv = 1;

            var user = _userManager.GetUserAsync(User).Result;

            var fakerProduct = new Faker<ProductModel>();
            fakerProduct.RuleFor(x => x.AuthorId, _ => user.Id);
            fakerProduct.RuleFor(x => x.Content, fk => fk.Commerce.ProductDescription() + "[FakeData]");
            fakerProduct.RuleFor(x => x.Description, fk => fk.Lorem.Sentences(3));
            fakerProduct.RuleFor(x => x.Published, _ => true);
            fakerProduct.RuleFor(x => x.Slug, fk => fk.Lorem.Slug());
            fakerProduct.RuleFor(x => x.Title, fk => $"Sản phẩm {bv++} : " + fk.Commerce.ProductName());
            fakerProduct.RuleFor(x => x.DateCreated,
                fk => fk.Date.Between(new DateTime(2022, 1, 1), new DateTime(2022, 12, 18)));
            fakerProduct.RuleFor(x => x.Price, fk => int.Parse(fk.Commerce.Price(500, 2000,0)));


            var products = new List<ProductModel>();

            for (var i = 0; i < 40; i++)
            {
                var product = fakerProduct.Generate();
                product.DateUpdated = product.DateCreated;
                var productCategoryProducts = new List<CategoryProduct> { categoryProducts[rCateIndex.Next(5)] };
                product.CategoryProducts = productCategoryProducts;
                products.Add(product);
            }
            //End Faker Product

            _dbContext.Products.AddRange(products);
            _dbContext.SaveChanges();
        }

        private void SeedPostCategory()
        {
            //Faker Category
            _dbContext.Categories.RemoveRange(_dbContext.Categories
                .Where(x => x.Description.Contains("[FakeData]")));
            _dbContext.Posts.RemoveRange(_dbContext.Posts
                .Where(x => x.Content.Contains("[FakeData]")));
            _dbContext.SaveChanges();


            var fakerCategory = new Faker<Category>();
            var cm = 1;
            fakerCategory.RuleFor(c => c.Title,
                fk => $"CM{cm++} " + fk.Lorem.Sentence(1, 2).Trim('.'));
            fakerCategory.RuleFor(c => c.Description,
                fk => fk.Lorem.Sentences(5) + "[FakeData]");
            fakerCategory.RuleFor(c => c.Slug,
                fk => fk.Lorem.Slug());

            var cate1 = fakerCategory.Generate();
            var cate11 = fakerCategory.Generate();
            var cate12 = fakerCategory.Generate();
            var cate2 = fakerCategory.Generate();
            var cate21 = fakerCategory.Generate();
            var cate211 = fakerCategory.Generate();

            cate11.ParentCategory = cate1;
            cate12.ParentCategory = cate1;
            cate21.ParentCategory = cate2;
            cate211.ParentCategory = cate21;

            var categories = new[] { cate1, cate11, cate2, cate12, cate21, cate211 };
            _dbContext.Categories.AddRange(categories);

            //Faker Post
            var rCateIndex = new Random();
            var bv = 1;

            var user = _userManager.GetUserAsync(User).Result;

            var fakerPost = new Faker<Post>();
            fakerPost.RuleFor(x => x.AuthorId, _ => user.Id);
            fakerPost.RuleFor(x => x.Content, fk => fk.Lorem.Paragraphs(7) + "[FakeData]");
            fakerPost.RuleFor(x => x.Description, fk => fk.Lorem.Sentences(3));
            fakerPost.RuleFor(x => x.Published, _ => true);
            fakerPost.RuleFor(x => x.Slug, fk => fk.Lorem.Slug());
            fakerPost.RuleFor(x => x.Title, fk => $"Bài {bv++} " + fk.Lorem.Sentence(3, 4).Trim('.'));
            fakerPost.RuleFor(x => x.DateCreated,
                fk => fk.Date.Between(new DateTime(2022, 1, 1), new DateTime(2022, 12, 18)));


            var posts = new List<Post>();

            for (var i = 0; i < 40; i++)
            {
                var post = fakerPost.Generate();
                post.DateUpdated = post.DateCreated;
                var postCategories = new List<Category> { categories[rCateIndex.Next(5)] };
                post.Categories = postCategories;
                posts.Add(post);
            }
            //End Faker Post

            _dbContext.Posts.AddRange(posts);
            _dbContext.SaveChanges();
        }
    }
}