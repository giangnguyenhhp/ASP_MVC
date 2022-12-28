using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

namespace ASP_MVC.Areas.AdminCP.Menu;

public class AdminSideBarService
{
    private readonly IUrlHelper _urlHelper;

    public List<SideBarItem> Items { get; set; } = new();

    public AdminSideBarService(IUrlHelperFactory factory, IActionContextAccessor accessor)
    {
        if (accessor.ActionContext != null) _urlHelper = factory.GetUrlHelper(accessor.ActionContext);
        //Khởi tạo Sidebar
        Items.Add(new SideBarItem() { Type = SideBarItemType.Divider });
        Items.Add(new SideBarItem() { Type = SideBarItemType.Heading, Title = "Quản lý chung" });
        Items.Add(new SideBarItem()
        {
            Type = SideBarItemType.NavItem,
            Title = "Quản lý Database",
            Area = "DatabaseManage",
            Controller = "Database",
            Action = "Index",
            AwSomeIcon = "<i class=\"fas fa-database\"></i>"
        });
        Items.Add(new SideBarItem()
        {
            Type = SideBarItemType.NavItem,
            Title = "Quản lý liên hệ",
            Area = "Contact",
            Controller = "Contact",
            Action = "Index",
            AwSomeIcon = "<i class=\"far fa-address-card\"></i>"
        });
        Items.Add(new SideBarItem() { Type = SideBarItemType.Divider });
        Items.Add(new SideBarItem()
        {
            Type = SideBarItemType.NavItem,
            Title = "Role & User",
            AwSomeIcon = "<i class=\"fas fa-folder-open\"></i>",
            CollapseId = "UserRole",
            Items = new List<SideBarItem>()
            {
                new()
                {
                    Type = SideBarItemType.NavItem,
                    Title = "Các vai trò (Role)",
                    Area = "Identity",
                    Controller = "Role",
                    Action = "Index",
                },
                new()
                {
                    Type = SideBarItemType.NavItem,
                    Title = "Tạo vai trò (Role)",
                    Area = "Identity",
                    Controller = "Role",
                    Action = "Create",
                },
                new()
                {
                    Type = SideBarItemType.NavItem,
                    Title = "Danh sách thành viên (User)",
                    Area = "Identity",
                    Controller = "User",
                    Action = "Index",
                },
            }
        });
        Items.Add(new SideBarItem() { Type = SideBarItemType.Divider });
        Items.Add(new SideBarItem()
        {
            Type = SideBarItemType.NavItem,
            Title = "Quản lý bài viết (Blog)",
            AwSomeIcon = "<i class=\"fas fa-folder-open\"></i>",
            CollapseId = "blog",
            Items = new List<SideBarItem>()
            {
                new()
                {
                    Type = SideBarItemType.NavItem,
                    Title = "Danh mục bài viết",
                    Area = "Blog",
                    Controller = "Category",
                    Action = "Index",
                },
                new()
                {
                    Type = SideBarItemType.NavItem,
                    Title = "Tạo danh mục bài viết",
                    Area = "Blog",
                    Controller = "Category",
                    Action = "Create",
                },
                new()
                {
                    Type = SideBarItemType.NavItem,
                    Title = "Danh sách bài viết",
                    Area = "Blog",
                    Controller = "Post",
                    Action = "Index",
                },
                new()
                {
                    Type = SideBarItemType.NavItem,
                    Title = "Tạo bài viết",
                    Area = "Blog",
                    Controller = "Post",
                    Action = "Create",
                },
            }
        });
        Items.Add(new SideBarItem() { Type = SideBarItemType.Divider });
        Items.Add(new SideBarItem()
        {
            Type = SideBarItemType.NavItem,
            Title = "Quản lý sản phẩm",
            AwSomeIcon = "<i class=\"fas fa-folder-open\"></i>",
            CollapseId = "product",
            Items = new List<SideBarItem>()
            {
                new()
                {
                    Type = SideBarItemType.NavItem,
                    Title = "Danh mục sản phẩm",
                    Area = "Product",
                    Controller = "CategoryProduct",
                    Action = "Index",
                },
                new()
                {
                    Type = SideBarItemType.NavItem,
                    Title = "Tạo danh mục sản phẩm",
                    Area = "Product",
                    Controller = "CategoryProduct",
                    Action = "Create",
                },
                new()
                {
                    Type = SideBarItemType.NavItem,
                    Title = "Danh sách sản phẩm",
                    Area = "Product",
                    Controller = "ProductManage",
                    Action = "Index",
                },
                new()
                {
                    Type = SideBarItemType.NavItem,
                    Title = "Tạo sản phẩm",
                    Area = "Product",
                    Controller = "ProductManage",
                    Action = "Create",
                },
            }
        });
        Items.Add(new SideBarItem() { Type = SideBarItemType.Divider });
        Items.Add(new SideBarItem()
        {
            Type = SideBarItemType.NavItem,
            Title = "Quản lý Files",
            Area = "Files",
            Controller = "FileManager",
            Action = "Index",
            AwSomeIcon = "<i class=\"fas fa-file-image\"></i>"
        });
    }

    public string RenderHtml()
    {
        var html = new StringBuilder();

        foreach (var item in Items)
        {
            html.Append(item.RenderHtml(_urlHelper));
        }

        return html.ToString();
    }

    public void SetActive(string Area, string Controller, string Action)
    {
        foreach (var sideBarItem in Items)
        {
            if (sideBarItem.Area == Area && sideBarItem.Controller == Controller && sideBarItem.Action == Action &&
                sideBarItem.Items == null)
            {
                sideBarItem.IsActive = true;
                return;
            }

            else if (sideBarItem.Items != null)
            {
                foreach (var childItem in sideBarItem.Items.Where(childItem => 
                             childItem.Area == Area &&
                             childItem.Controller == Controller &&
                             childItem.Action == Action))
                {
                    childItem.IsActive = true;
                    sideBarItem.IsActive = true;
                }
            }
        }
    }
}