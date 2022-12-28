using ASP_MVC.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASP_MVC.Areas.AdminCP.Controllers;
[Area("AdminCP")]
[Authorize(Roles = RoleName.Administrator)]
public class AdminControlPanel : Controller
{
    [Route("/admincp")]
    public IActionResult Index() => View();
}