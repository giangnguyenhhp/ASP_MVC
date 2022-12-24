using System.ComponentModel.DataAnnotations;

namespace ASP_MVC.Models.Product;

public class ProductPhoto
{
    [Key]
    public int Id { get; set; }

    //abc.jgp,123.png,...
    // /Uploads/Products/abc.jpg
    public string FileName { get; set; }
    

    public ProductModel? Product { get; set; }
}