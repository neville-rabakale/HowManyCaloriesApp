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

        public StartController (ApplicationDbContext context)
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

            //Add inputed userdata to Db and Save
            _context.UserProfiles.Add(profile);
            TempData["Success"] = "Start data created Successfully";
            _context.SaveChanges();
            return RedirectToAction("Week1", "Weeks");
          
        }


    }
}
