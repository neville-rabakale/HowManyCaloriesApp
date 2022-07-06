using HowManyCalories.Data;
using HowManyCalories.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace HowManyCalories.Controllers
{
    public class StartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StartController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            UserProfile profile = new();

            return View(profile);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Index(UserProfile profile)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            profile.ApplicationUserId = claim.Value;

            //Before we start, we need to check if the goal is realisitc
            //Check if the weight loss is less than 1% body weight per week
            var weeklyLoss = (profile.StartWeight - profile.GoalWeight) / (profile.Duration - 1);
            if(weeklyLoss > (profile.GoalWeight * 0.01))
            {
                TempData["error"] = "You are restricted to losing 1% of your body weight per week, Please increase your duration or decrease your goal weight";
                return RedirectToAction("Index");
            }
            //Add inputed userdata to Db and Save
            _context.UserProfiles.Add(profile);
            TempData["Success"] = "Start data created Successfully";
            _context.SaveChanges();
            return RedirectToAction("Week1", "Weeks");

        }

    }
}
