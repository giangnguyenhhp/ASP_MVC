using System.ComponentModel;
using ASP_MVC.Models.Blog;

namespace ASP_MVC.Areas.Blog.Models;

public class CreatePostModel : Post
{
    [DisplayName("Chuyên mục")]
    public int[]? CategoriesId { get; set; }
}