using System.ComponentModel;
using ASP_MVC.Models.Blog;
using ASP_MVC.Models.Product;

namespace ASP_MVC.Areas.Product.Models;

public class CreateProductModel : ProductModel
{
    [DisplayName("Chuyên mục")]
    public int[]? CategoryProductIds { get; set; }
}