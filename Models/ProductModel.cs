using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ASP_MVC.Models;

public class ProductModel : PageModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public double Price { get; set; }
}