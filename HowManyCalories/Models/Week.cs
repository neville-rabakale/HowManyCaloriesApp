using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HowManyCalories.Models
{
    public class Week
    {

        public int Id { get; set; }
        [Required]
        [Display(Name = "Week")]
        public int WeekNumber { get; set; } = 0;
        [Display(Name = "Target Weight")]
        public double ExpectedWeight { get; set; }
        [Display(Name = "Average Weight")]
        public double AverageWeight { get; set; }
        [Display(Name = "Calories")]
        public double WeeklyCalories { get; set; }
        public double CurrentCalories { get; set; }
        [Display(Name ="Total Weight Loss")]
        public double WeeklyLoss { get; set; }
        [Required]
        [Display(Name = "Check in 1")]
        public double CheckIn1 { get; set; }
        [Required]
        [Display(Name = "Check in 2")]
        public double CheckIn2 { get; set; }
        [Required]
        [Display(Name = "Check in 3")]
        public double CheckIn3 { get; set; }
        public int UserProfileId { get; set; }
        [ForeignKey("UserProfileId")]
        [ValidateNever]
        public UserProfile UserProfile { get; set; }

    }
}
