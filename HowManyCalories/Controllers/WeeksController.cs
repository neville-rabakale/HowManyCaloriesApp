using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HowManyCalories.Data;
using HowManyCalories.Models;
using System.Linq.Expressions;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace HowManyCalories.Controllers
{
    public class WeeksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WeeksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Weeks
        public async Task<IActionResult> Index()
        {
              return _context.Weeks != null ? 
                          View(await _context.Weeks.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Weeks'  is null.");
        }

        //Week 1 Get
        public IActionResult Week1()
        {

            Week week = CreateWeek();

            //This is the initial week/Week 1.
            if( week.WeekNumber == 0)
            {
                week.UserProfileId = week.UserProfile.Id;
                week.WeekNumber = 1;
                week.WeeklyLoss = WeeklyLoss(week.UserProfile.StartWeight, week.UserProfile.StartWeight, 0); 
                week.ExpectedWeight = week.UserProfile.StartWeight;
                week.AverageWeight = week.UserProfile.StartWeight;
                week.CurrentCalories = week.UserProfile.StartCalories;
                //Start Calories - 10% of start calories
                week.WeeklyCalories = week.UserProfile.StartCalories - (week.UserProfile.StartCalories * 0.1);
            }
            return View(week);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Week1(Week week1)
        { 
            //Add inputed userdata to Db and Save
            _context.Weeks.Add(week1);
            TempData["Success"] = "Week 1 data added Successfully";
            _context.SaveChanges();
            return RedirectToAction("Week2");

        }

        //Week 2 Get
        public IActionResult Week2()
        {
            //week 2 init
            Week week2 = CreateWeek();
            // we need to pull week 1 from db
            var weekFromDb = GetFirstOrDefaultWeek(u => u.WeekNumber == 1);
            //and make sure we pull the week 1 record
            if (weekFromDb.WeekNumber == 1) //--this could alse be written backwards, if(weekNum != 1){error and return} else {do somthing}
            {
                week2.WeekNumber = 2;
                week2.UserProfileId = weekFromDb.UserProfile.Id;
                week2.AverageWeight = AverageCheckinWeight(weekFromDb.CheckIn1, weekFromDb.CheckIn2, weekFromDb.CheckIn3);
                week2.WeeklyLoss = WeeklyLoss(weekFromDb.UserProfile.StartWeight, week2.AverageWeight, 0); //still 0
                week2.ExpectedWeight = ExpectedLoss(weekFromDb.UserProfile.StartWeight, weekFromDb.UserProfile.StartWeight, weekFromDb.UserProfile.GoalWeight, weekFromDb.UserProfile.Duration);
                week2.CurrentCalories = weekFromDb.WeeklyCalories;
                week2.WeeklyCalories = WeeklyCal(weekFromDb.ExpectedWeight, weekFromDb.AverageWeight, weekFromDb.WeeklyCalories);
                //If we need to add checkin then they are 0
            }
            return View(week2);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Week2(Week week2)
        {
            //Add inputed userdata to Db and Save
            _context.Weeks.Add(week2);
            TempData["Success"] = "Week 1 data added Successfully";
            _context.SaveChanges();
            return RedirectToAction("Week3");

        }
        //Week 2 Get
        public IActionResult Week3()
        {

            Week week3 = CreateWeek();

            //This is week2
            // we need to pull week 1 from db
            var weekFromDb = GetFirstOrDefaultWeek(u => u.WeekNumber == 2);
            //and make sure we pull the week 1 record
            if (weekFromDb.WeekNumber == 2)
            {
                week3.WeekNumber = 3;
                week3.UserProfileId = weekFromDb.UserProfile.Id;
                week3.AverageWeight = AverageCheckinWeight(weekFromDb.CheckIn1, weekFromDb.CheckIn2, weekFromDb.CheckIn3);
                week3.WeeklyLoss = WeeklyLoss(weekFromDb.AverageWeight, week3.AverageWeight, weekFromDb.WeeklyLoss);
                week3.ExpectedWeight = ExpectedLoss(weekFromDb.ExpectedWeight, weekFromDb.UserProfile.StartWeight, weekFromDb.UserProfile.GoalWeight, weekFromDb.UserProfile.Duration);
                week3.CurrentCalories = weekFromDb.WeeklyCalories;
                week3.WeeklyCalories = WeeklyCal(weekFromDb.ExpectedWeight, weekFromDb.AverageWeight, weekFromDb.WeeklyCalories);
                //If we need to add checkin then they are 0
            }
            return View(week3);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Week3(Week week3)
        {
            //Add inputed userdata to Db and Save
            _context.Weeks.Add(week3);
            TempData["Success"] = "Week 2 data added Successfully";
            _context.SaveChanges();
            return RedirectToAction("Week4");

        }
        //Week 4 Get
        public IActionResult Week4()
        {
            Week week4 = CreateWeek();
            var weekFromDb = GetFirstOrDefaultWeek(u => u.WeekNumber == 3);
            if (weekFromDb.WeekNumber == 3)
            {
                week4.WeekNumber += 1;
                week4.UserProfileId = weekFromDb.UserProfile.Id;
                week4.AverageWeight = AverageCheckinWeight(weekFromDb.CheckIn1, weekFromDb.CheckIn2, weekFromDb.CheckIn3);
                week4.WeeklyLoss = WeeklyLoss(weekFromDb.AverageWeight, week4.AverageWeight, weekFromDb.WeeklyLoss);
                week4.ExpectedWeight = ExpectedLoss(weekFromDb.ExpectedWeight, weekFromDb.UserProfile.StartWeight, weekFromDb.UserProfile.GoalWeight, weekFromDb.UserProfile.Duration);
                week4.CurrentCalories = weekFromDb.WeeklyCalories;
                week4.WeeklyCalories = WeeklyCal(weekFromDb.ExpectedWeight, weekFromDb.AverageWeight, weekFromDb.WeeklyCalories);
                //If we need to add checkin then they are 0
            }
            return View(week4);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Week4(Week week4)
        {
            //Add inputed userdata to Db and Save
            _context.Weeks.Add(week4);
            TempData["Success"] = "Week 2 data added Successfully";
            _context.SaveChanges();
            return RedirectToAction("Week5");

        }



        //Create mew week instance with current user Id
        public Week CreateWeek()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            Week week = new()
            {
                UserProfile = GetFirstOrDefaultProfile(u => u.ApplicationUserId == claim.Value, includeProperties: "ApplicationUser"),
            };
            return week;
        }

        //How much did you actually lose in total
        double WeeklyLoss( double previous, double current, double total)
        {   
            var currentLoss = previous - current;

            return currentLoss += total;
        }

        //Calculate expected weekly weight loss
        double ExpectedLoss( double current, double startWeight, double goalWeight, double time)
        {
            var totalLoss = startWeight - goalWeight;
            var weeklyLoss = totalLoss / (time - 1);
            var expectedLoss = current - weeklyLoss;
            return expectedLoss;

        }

        //calculate weekly calories
        double WeeklyCal(double expectedWeight, double averageWeight, double calories)
        {
            //if averge weight <= 2% of your expected weight
            if((averageWeight - expectedWeight) >= (expectedWeight * 0.2))
            {
                //subtract calories by 10%
                var weeklycalories = (calories - (calories * 0.1));
                return weeklycalories;
            }
            return calories;
            //else do nothing
        }

        //calculate average weight
        double AverageCheckinWeight(double checkIn1, double checkIn2, double checkIn3)
        {
            double avg = ((checkIn1 + checkIn2 + checkIn3) / 3);
            return avg;
        }

        //Get Item for Weeks Query based on a condition
        public Week GetFirstOrDefaultWeek (Expression<Func<Week, bool>> filter, string? includeProperties = null)
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

        //Get All Query for Weeks
        public IEnumerable<Week> GetAllWeeks(Expression<Func<Week, bool>>? filter = null, string? includeProperties = null)
        {

            //First we need to query the db
            IQueryable<Week> query = _context.Weeks;
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

    }
}
