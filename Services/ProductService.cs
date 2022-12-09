using ASP_MVC.Models;

namespace ASP_MVC.Services;

public class ProductService : List<ProductModel>
{
    public ProductService()
    {
        AddRange(new ProductModel[]
        {
            new(){Id = 1,Name = "Iphone X",Price = 1000},
            new(){Id = 2,Name = "Iphone Xs",Price = 2000},
            new(){Id = 3,Name = "IPhone Xs-Max",Price = 500},
            new(){Id = 4,Name = "IPhone 14",Price = 1200},
        });
    }
}