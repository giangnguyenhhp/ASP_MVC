﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ASP_MVC.Models.Blog;

namespace ASP_MVC.Models.Product;

[Table("CategoryProduct")]
public class CategoryProduct
{
    [Key] public int Id { get; set; }


    // Tiều đề CategoryProduct
    [Required(ErrorMessage = "Phải có tên danh mục")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "{0} dài {1} đến {2}")]
    [Display(Name = "Tên danh mục")]
    public string Title { get; set; }

    // Nội dung, thông tin chi tiết về Category
    [DataType(DataType.Text)]
    [Display(Name = "Nội dung danh mục")]
    public string Description { set; get; }

    //chuỗi Url
    [Required(ErrorMessage = "Phải tạo url")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "{0} dài {1} đến {2}")]
    [RegularExpression(@"^[a-z0-9-]*$", ErrorMessage = "Chỉ dùng các ký tự [a-z0-9-]")]
    [Display(Name = "Url hiện thị")]
    public string Slug { set; get; }

    // Các Category con
    public ICollection<CategoryProduct>? CategoryChildren { get; set; }

    // Category cha (FKey)
    [Display(Name = "ID danh mục cha")] public int? ParentCategoryId { get; set; }

    [ForeignKey("ParentCategoryId")]
    [Display(Name = "Danh mục cha")]
    public CategoryProduct? ParentCategory { set; get; }

    public List<ProductModel>? Products { get; set; }

    /// <summary>
    /// Lấy ra list Id của CategoryChildren
    /// </summary>
    /// <param name="lists"></param>
    /// <param name="childCategories"></param>
    public void ChildCategoryIds(List<int> lists, ICollection<CategoryProduct>? childCategories = null)
    {
        childCategories ??= CategoryChildren;

        if (childCategories == null) return;
        foreach (var childCategory in childCategories)
        {
            lists.Add(childCategory.Id);
            ChildCategoryIds(lists, childCategory.CategoryChildren);
        }
    }

    public List<CategoryProduct> ListParents()
    {
        var list = new List<CategoryProduct>();
        var parent = ParentCategory;
        while (parent != null)
        {
            list.Add(parent);
            parent = parent.ParentCategory;
        }

        list.Reverse();
        return list;
    }
}