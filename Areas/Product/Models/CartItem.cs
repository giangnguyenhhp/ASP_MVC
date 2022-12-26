using ASP_MVC.Models.Product;

namespace ASP_MVC.Areas.Product.Models;

public class CartItem
{
    public int quantity { get; set; }

    public ProductModel ProductModel { get; set; }
}