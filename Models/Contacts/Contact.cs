using System.ComponentModel.DataAnnotations;

namespace ASP_MVC.Models.Contacts;

public class Contact
{
    [Key]
    public int Id { get; set; }
    
    [StringLength(50)]
    [Required(ErrorMessage = "Phải nhập {0}")]
    [Display(Name = "Họ Tên")]
    public string FullName { get; set; }

    [EmailAddress(ErrorMessage = "Phải là {0}")]
    [Required(ErrorMessage = "Phải nhập {0}")]
    [Display(Name = "Địa chỉ Email")]
    public string Email { get; set; }

    public DateTime DateSent { get; set; }

    [Display(Name = "Nội dung")]
    public string Message { get; set; }

    [Phone(ErrorMessage = "Phải là số điện thoại")]
    [StringLength(50)]
    [Display(Name = "Số điện thoại")]
    public string Phone { get; set; }
}
