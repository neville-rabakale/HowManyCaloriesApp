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
            return View();
        }
        [HttpPost]
        public IActionResult PostIndex(UserProfile obj)
        {
            //Add inputed userdata to Db and Save
            _context.UserProfiles.Add(obj);
            TempData["Success"] = "Start data created Successfully";
            _context.SaveChanges();
            //this should redirect to Action Index of week 1 Controller
            return View();
        }

    }
}
