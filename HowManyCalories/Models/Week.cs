using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HowManyCalories.Models
{
    public class Week
    {
        [Key]
        [Required]
        public int WeekNumber { get; set; }
        public double ExpectedWeight { get; set; }
        public double AverageWeight { get; set; }
        public int WeeklyCalories { get; set; }
        public int CurrentCalories { get; set; }
        public double CheckIn1 { get; set; }
        public double CheckIn2 { get; set; }
        public double CheckIn3 { get; set; }

    }
}
