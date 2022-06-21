using HowManyCalories.Data;
using HowManyCalories.Models;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult IndexPOST(UserProfile obj)
        {

            if (ModelState.IsValid) {
                //Add inputed userdata to Db and Save
                _context.UserProfiles.Add(obj);
                TempData["Success"] = "Start data created Successfully";
                _context.SaveChanges();
                return RedirectToAction("Week1", "Weeks");

            }
            return View(obj);
        }

    }
}
