using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace ASP_MVC.Areas.AdminCP.Menu;

public enum SideBarItemType
{
    Divider,
    Heading,
    NavItem
}

public class SideBarItem
{
    public string Title { get; set; }

    public bool IsActive { get; set; }

    public SideBarItemType Type { get; set; }

    public string Area { get; set; }

    public string Controller { get; set; }

    public string Action { get; set; }

    public string? AwSomeIcon { get; set; } // fas fa-fw fa-cog

    public List<SideBarItem>? Items { get; set; }

    public string CollapseId { get; set; }

    public string? GetLink(IUrlHelper urlHelper)
    {
        return urlHelper.Action(Action, Controller, new { area = Area });
    }

    public string RenderHtml(IUrlHelper urlHelper)
    {
        var html = new StringBuilder();
        switch (Type)
        {
            case SideBarItemType.Divider:
                html.Append("<hr class=\"sidebar-divider my-2\">");
                break;
            case SideBarItemType.Heading:
                html.Append($"<div class=\"sidebar-heading\">{Title}</div>");
                break;
            case SideBarItemType.NavItem:
                if (Items == null)
                {
                    var url = GetLink(urlHelper);
                    
                    var cssClass = "nav-item";
                    if(IsActive) cssClass += " active";
                    
                    var icon = (AwSomeIcon != null) ? $"{AwSomeIcon}" : "";
                    
                    html.Append($@"<li class=""{cssClass}"">
                         <a class=""nav-link"" href=""{url}"">
                        {icon}
                        <span>{Title}</span>
                        </a>
                        </li>");
                }
                else
                {
                    //Item!=null
                    var cssClass = "nav-item";
                    if(IsActive) cssClass += " active";
                    
                    var icon = (AwSomeIcon != null) ? $"{AwSomeIcon}" : "";
                    
                    var collapseCssClass = "collapse";
                    if (IsActive) cssClass += " show";
                    
                    var itemMenu = "";
                    foreach (var item in Items)
                    {
                        var urlItem = item.GetLink(urlHelper);
                        var cssItemClass = "collapse-item";
                        if (item.IsActive) cssItemClass += " active";
                        itemMenu += $"<a class=\"{cssItemClass}\" href=\"{urlItem}\">{item.Title}</a>";
                    }

                    html.Append($@"<li class=""{cssClass}"">
                        <a class=""nav-link collapsed"" href=""#"" data-toggle=""collapse"" data-target=""#{CollapseId}""
                                aria-expanded=""true"" aria-controls=""{CollapseId}"">
                            {icon}
                            <span>{Title}</span>
                        </a>
                        <div id=""{CollapseId}"" class=""{collapseCssClass}"" aria-labelledby=""headingTwo"" data-parent=""#accordionSidebar"">
                            <div class=""bg-white py-2 rounded collapse-inner"">
                                {itemMenu}
                            </div>
                        </div>
                        </li>");
                }

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }


        return html.ToString();
    }
}