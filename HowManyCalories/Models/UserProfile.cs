using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace HowManyCalories.Models
{
    public class UserProfile
    {
        public int Id { get; set; }
        public double StartWeight { get; set; }
        public double GoalWeight { get; set; }


    }
}
