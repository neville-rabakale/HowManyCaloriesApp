using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HowManyCalories.Models
{
    public class UserProfile
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name="Start Weight")]
        public double StartWeight { get; set; }
        [Display(Name = "Goal Weight")]
        [Required]
        public double GoalWeight { get; set; }
        [Display(Name = "Start Calories")]
        [Required]
        public double StartCalories { get; set; }
        [Required]
        public int Duration { get; set; }
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }


    }
}
