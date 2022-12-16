using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ASP_MVC.Models 
{
    public class AppUser: IdentityUser 
    {
          [StringLength(400)]  
          public string? HomeAdress { get; set; }

          // [Required]       
          [DataType(DataType.Date)]
          public DateTime? BirthDate { get; set; }
    }
}
