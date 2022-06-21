using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace HowManyCalories.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string Name { get; set; }
    }
}
