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
        public IActionResult Continue()
        {
            UserProfile uProfile = new();
            Week week = new();

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var profileFromDb = GetAllProfiles(u => u.ApplicationUserId == claim.Value, includeProperties: "ApplicationUser");
            //Check if there are multiple UserProfile Id's for this User
            if(profileFromDb != null)
            {
                if(profileFromDb.Count() >= 1)
                {
                    //If yes, get the largest id in the UserProfileId row -- Can turn this into a function called GetProfileId --
                    var profileIds = _context.UserProfiles
                        .Where(u => u.ApplicationUserId == claim.Value)
                        .Select(u => u.Id)
                        .ToList();
                    uProfile = GetFirstOrDefaultProfile(u=> u.Id == profileIds.Max());

                    //Before we move forward we need to retrieve the weeks of this profile and 
                    //Return/Goto that particular week
                    var weekFromDb = _context.Weeks
                       .Where(u => u.UserProfile.ApplicationUserId == claim.Value)
                       .Select(u => u.WeekNumber)
                       .ToList();

                    week = GetFirstOrDefaultWeek(u => u.UserProfile.ApplicationUserId == claim.Value && u.WeekNumber == weekFromDb.Max());
                 //   week.WeekNumber = weekFromDb.Max(); // need to get the week for the correct profileId

                    return RedirectToWeek((week.WeekNumber + 1));
                    //We can also combine this action with the index action 

                }

            }

            return RedirectToAction("Index");
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

        //GetAll Query for UserProfiles
        public IEnumerable<UserProfile> GetAllProfiles(Expression<Func<UserProfile, bool>>? filter = null, string? includeProperties = null)
        {

            //First we need to query the db
            IQueryable<UserProfile> query = _context.UserProfiles;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (includeProperties != null)
            {
                //first split "includeProperties" by ','
                foreach (var property in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    //include all the propery results to the query
                    query = query.Include(property);

                }
            }
            //then return it as a list
            return query.ToList();

        }
        //Get Item for Weeks Query based on a condition
        public UserProfile GetFirstOrDefaultProfile(Expression<Func<UserProfile, bool>> filter, string? includeProperties = null)
        {
            //First we need to query the db
            IQueryable<UserProfile> query = _context.UserProfiles;
            query = query.Where(filter);
            if (includeProperties != null)
            {
                //first split "includeProperties" by ','
                foreach (var property in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    //include all the propery results to the query
                    query = query.Include(property);

                }
            }

            //then return it as a list
            return query.FirstOrDefault();
        }

        //Get Item for Weeks Query based on a condition
        public Week GetFirstOrDefaultWeek(Expression<Func<Week, bool>> filter, string? includeProperties = null)
        {
            //First we need to query the db
            IQueryable<Week> query = _context.Weeks;
            query = query.Where(filter);
            if (includeProperties != null)
            {
                //first split "includeProperties" by ','
                foreach (var property in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    //include all the propery results to the query
                    query = query.Include(property);

                }
            }

            //then return it as a list
            return query.FirstOrDefault();
        }
        //Returns Action to particular weekNumber
        public IActionResult RedirectToWeek(int weekNumber)
        {
            if( weekNumber == 1)
            {
                return RedirectToAction("Week1", "Weeks");
            }
            if (weekNumber == 2)
            {
                return RedirectToAction("Week2", "Weeks");
            }
            if (weekNumber == 3)
            {
                return RedirectToAction("Week3", "Weeks");
            }
            if (weekNumber == 4)
            {
                return RedirectToAction("Week4", "Weeks");
            }
            if (weekNumber == 5)
            {
                return RedirectToAction("Week5", "Weeks");
            }
            if (weekNumber == 6)
            {
                return RedirectToAction("Week6", "Weeks");
            }
            if (weekNumber == 7)
            {
                return RedirectToAction("Week7", "Weeks");
            }
            if (weekNumber == 8)
            {
                return RedirectToAction("Week8", "Weeks");
            }
            if (weekNumber == 9)
            {
                return RedirectToAction("Week9", "Weeks");
            }
            if (weekNumber == 10)
            {
                return RedirectToAction("Week10", "Weeks");
            }
            if (weekNumber == 11)
            {
                return RedirectToAction("Week11", "Weeks");
            }
            else 
            {
                return RedirectToAction("Week12", "Weeks");
            }
        } 

    }
}
