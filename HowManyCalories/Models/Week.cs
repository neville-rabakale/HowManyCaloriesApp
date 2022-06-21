using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HowManyCalories.Models
{
    public class Week
    {

        public int Id { get; set; }
        [Required]
        public int WeekNumber { get; set; }
        public double ExpectedWeight { get; set; }
        public double AverageWeight { get; set; }
        public int WeeklyCalories { get; set; }
        public int CurrentCalories { get; set; }
        [Required]
        [Display(Name = "Check in 1")]
        public double CheckIn1 { get; set; }
        [Required]
        [Display(Name = "Check in 2")]
        public double CheckIn2 { get; set; }
        [Required]
        [Display(Name = "Check in 3")]
        public double CheckIn3 { get; set; }

    }
}
