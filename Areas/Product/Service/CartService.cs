using ASP_MVC.Areas.Product.Models;
using Newtonsoft.Json;

namespace ASP_MVC.Areas.Product.Service;

public class CartService
{
    //Key lưu trữ chuỗi Json của cart
    public const string CARTKEY = "cart";

    private readonly IHttpContextAccessor _contextAccessor;

    private readonly HttpContext? _httpContext;

    public CartService(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
        _httpContext = contextAccessor.HttpContext;
    }

    // Lấy Cart từ Session (danh sách CartItem)
    public List<CartItem>? GetCartItems()
    {
        var session = _httpContext?.Session;
        var jsonCart = session?.GetString(CARTKEY);
        return jsonCart != null ? JsonConvert.DeserializeObject<List<CartItem>>(jsonCart) : new List<CartItem>();
    }
    
    //Xóa Cart khỏi Session
    public void ClearCart()
    {
        var session = _httpContext?.Session;
        session?.Remove(CARTKEY);
    }
    
    //Lưu Cart (Danh sách CartItem) vào session
    public void SaveCartSession(List<CartItem> cartItems)
    {
        var session = _httpContext?.Session;
        var jsonCart = JsonConvert.SerializeObject(cartItems);
        session?.SetString(CARTKEY, jsonCart);
    }
}