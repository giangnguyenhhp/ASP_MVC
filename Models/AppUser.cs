using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ASP_MVC.Models;

public class AppUser : IdentityUser
{
    [StringLength(400)]
    public string HomeAddress { get; set; }
    
    public string? BirthDate { get; set; }
}