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
        public IActionResult Summary()
        {
            return View();
        }

        //Week 1 Get
        public IActionResult Week1()
        {

            Week week = CreateWeek();

            //This is the initial week/Week 1.
            week.WeekNumber = 0;
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
            TempData["Success"] = "Week 1 Complted Successfully, Well done";
            _context.SaveChanges();
            return RedirectToAction("Week2");

        }

        //Week 2 Get
        public IActionResult Week2()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            //week 2 init
            Week week2 = CreateWeek();
            // we need to pull week 1 from db
            var weekFromDb = GetFirstOrDefaultWeek(u => u.WeekNumber == 1 && u.UserProfile.ApplicationUserId == claim.Value);
            //and make sure we pull the week 1 record
            if (weekFromDb.WeekNumber == 1) //--this could alse be written backwards, if(weekNum != 1){error and return} else {do somthing}
            {
                week2.UserProfileId = weekFromDb.UserProfile.Id;
                week2.AverageWeight = AverageCheckinWeight(weekFromDb.CheckIn1, weekFromDb.CheckIn2, weekFromDb.CheckIn3);
                week2.WeeklyLoss = WeeklyLoss(weekFromDb.UserProfile.StartWeight, week2.AverageWeight, 0); //still 0
                week2.ExpectedWeight = ExpectedLoss(weekFromDb.UserProfile.StartWeight, weekFromDb.UserProfile.StartWeight, weekFromDb.UserProfile.GoalWeight, weekFromDb.UserProfile.Duration);
                week2.CurrentCalories = weekFromDb.WeeklyCalories;
                week2.WeeklyCalories = WeeklyCal(weekFromDb.ExpectedWeight, weekFromDb.AverageWeight, weekFromDb.WeeklyCalories);
                week2.WeekNumber = weekFromDb.WeekNumber += 1;

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
            TempData["Success"] = "Week 2 Completed Successfully";
            _context.SaveChanges();
            return RedirectToAction("Week3");

        }
        //Week 2 Get
        public IActionResult Week3()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            Week week3 = CreateWeek();

            //This is week2
            // we need to pull week 1 from db
            var weekFromDb = GetFirstOrDefaultWeek(u => u.WeekNumber == 2 && u.UserProfile.ApplicationUserId == claim.Value);
            //and make sure we pull the week 2 record
            if (weekFromDb.WeekNumber == 2)
            {
                week3.WeekNumber = weekFromDb.WeekNumber += 1;
                week3.UserProfileId = weekFromDb.UserProfile.Id;
                week3.AverageWeight = AverageCheckinWeight(weekFromDb.CheckIn1, weekFromDb.CheckIn2, weekFromDb.CheckIn3);
                week3.WeeklyLoss = WeeklyLoss(weekFromDb.AverageWeight, week3.AverageWeight, weekFromDb.WeeklyLoss);
                week3.ExpectedWeight = ExpectedLoss(weekFromDb.ExpectedWeight, weekFromDb.UserProfile.StartWeight, weekFromDb.UserProfile.GoalWeight, weekFromDb.UserProfile.Duration);
                week3.CurrentCalories = weekFromDb.WeeklyCalories;
                week3.WeeklyCalories = WeeklyCal(weekFromDb.ExpectedWeight, weekFromDb.AverageWeight, weekFromDb.WeeklyCalories);
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
            TempData["Success"] = "Week 3 Completed Successfully";
            _context.SaveChanges();
            return RedirectToAction("Week4");

        }
        //Week 4 Get
        public IActionResult Week4()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            Week week4 = CreateWeek();
            var weekFromDb = GetFirstOrDefaultWeek(u => u.WeekNumber == 3 && u.UserProfile.ApplicationUserId == claim.Value);
            if (weekFromDb.WeekNumber == 3)
            {
                week4.WeekNumber = (weekFromDb.WeekNumber += 1);
                week4.UserProfileId = weekFromDb.UserProfile.Id;
                week4.AverageWeight = AverageCheckinWeight(weekFromDb.CheckIn1, weekFromDb.CheckIn2, weekFromDb.CheckIn3);
                week4.WeeklyLoss = WeeklyLoss(weekFromDb.AverageWeight, week4.AverageWeight, weekFromDb.WeeklyLoss);
                week4.ExpectedWeight = ExpectedLoss(weekFromDb.ExpectedWeight, weekFromDb.UserProfile.StartWeight, weekFromDb.UserProfile.GoalWeight, weekFromDb.UserProfile.Duration);
                week4.CurrentCalories = weekFromDb.WeeklyCalories;
                week4.WeeklyCalories = WeeklyCal(weekFromDb.ExpectedWeight, weekFromDb.AverageWeight, weekFromDb.WeeklyCalories);
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
            TempData["Success"] = "Week 4 Completed Successfully";
            _context.SaveChanges();
            return RedirectToAction("Week5");

        }
        //Week 4 Get
        public IActionResult Week5()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            Week week5 = CreateWeek();
            var weekFromDb = GetFirstOrDefaultWeek(u => u.WeekNumber == 4 && u.UserProfile.ApplicationUserId == claim.Value);
            if (weekFromDb.WeekNumber == 4)
            {
                week5.WeekNumber = (weekFromDb.WeekNumber += 1);
                week5.UserProfileId = weekFromDb.UserProfile.Id;
                week5.AverageWeight = AverageCheckinWeight(weekFromDb.CheckIn1, weekFromDb.CheckIn2, weekFromDb.CheckIn3);
                week5.WeeklyLoss = WeeklyLoss(weekFromDb.AverageWeight, week5.AverageWeight, weekFromDb.WeeklyLoss);
                week5.ExpectedWeight = ExpectedLoss(weekFromDb.ExpectedWeight, weekFromDb.UserProfile.StartWeight, weekFromDb.UserProfile.GoalWeight, weekFromDb.UserProfile.Duration);
                week5.CurrentCalories = weekFromDb.WeeklyCalories;
                week5.WeeklyCalories = WeeklyCal(weekFromDb.ExpectedWeight, weekFromDb.AverageWeight, weekFromDb.WeeklyCalories);
            }
            return View(week5);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Week5(Week week5)
        {
            //Add inputed userdata to Db and Save
            _context.Weeks.Add(week5);
            TempData["Success"] = "Week 5 Completed Successfully";
            _context.SaveChanges();
            return RedirectToAction("Week6");

        }
        //Week 4 Get
        public IActionResult Week6()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            Week week6 = CreateWeek();
            var weekFromDb = GetFirstOrDefaultWeek(u => u.WeekNumber == 5 && u.UserProfile.ApplicationUserId == claim.Value);
            if (weekFromDb.WeekNumber == 5)
            {
                week6.WeekNumber = (weekFromDb.WeekNumber += 1);
                week6.UserProfileId = weekFromDb.UserProfile.Id;
                week6.AverageWeight = AverageCheckinWeight(weekFromDb.CheckIn1, weekFromDb.CheckIn2, weekFromDb.CheckIn3);
                week6.WeeklyLoss = WeeklyLoss(weekFromDb.AverageWeight, week6.AverageWeight, weekFromDb.WeeklyLoss);
                week6.ExpectedWeight = ExpectedLoss(weekFromDb.ExpectedWeight, weekFromDb.UserProfile.StartWeight, weekFromDb.UserProfile.GoalWeight, weekFromDb.UserProfile.Duration);
                week6.CurrentCalories = weekFromDb.WeeklyCalories;
                week6.WeeklyCalories = WeeklyCal(weekFromDb.ExpectedWeight, weekFromDb.AverageWeight, weekFromDb.WeeklyCalories);
            }
            return View(week6);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Week6(Week week6)
        {
            //Add inputed userdata to Db and Save
            _context.Weeks.Add(week6);
            _context.SaveChanges();

            //check if duration of diet is 6 weeks, if so goto summary else continue
            if (week6.UserProfile.Duration == 6)
            {
                week6.UserProfile.Duration = 0;
                _context.Weeks.Add(week6);
                _context.SaveChanges();
                TempData["Success"] = "Great Job, you have successully completed your 6 week weight loss program";
                //You are at the end of the diet
                return RedirectToAction("Summary");
            }
            TempData["Success"] = "Week 6 Completed Successfully, Keep it going!";
            return RedirectToAction("Week7");

        }
        //Week 7 Get
        public IActionResult Week7()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            Week week7 = CreateWeek();
            var weekFromDb = GetFirstOrDefaultWeek(u => u.WeekNumber == 6 && u.UserProfile.ApplicationUserId == claim.Value);
            if (weekFromDb.WeekNumber == 6)
            {
                week7.WeekNumber = (weekFromDb.WeekNumber += 1);
                week7.UserProfileId = weekFromDb.UserProfile.Id;
                week7.AverageWeight = AverageCheckinWeight(weekFromDb.CheckIn1, weekFromDb.CheckIn2, weekFromDb.CheckIn3);
                week7.WeeklyLoss = WeeklyLoss(weekFromDb.AverageWeight, week7.AverageWeight, weekFromDb.WeeklyLoss);
                week7.ExpectedWeight = ExpectedLoss(weekFromDb.ExpectedWeight, weekFromDb.UserProfile.StartWeight, weekFromDb.UserProfile.GoalWeight, weekFromDb.UserProfile.Duration);
                week7.CurrentCalories = weekFromDb.WeeklyCalories;
                week7.WeeklyCalories = WeeklyCal(weekFromDb.ExpectedWeight, weekFromDb.AverageWeight, weekFromDb.WeeklyCalories);
            }
            return View(week7);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Week7(Week week7)
        {
            //Add inputed userdata to Db and Save
            _context.Weeks.Add(week7);
            TempData["Success"] = "Week 7 completed Successfully";
            _context.SaveChanges();
            return RedirectToAction("Week8");

        }
        //Week 8 Get
        public IActionResult Week8()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            Week week8 = CreateWeek();
            var weekFromDb = GetFirstOrDefaultWeek(u => u.WeekNumber == 7 && u.UserProfile.ApplicationUserId == claim.Value);
            if (weekFromDb.WeekNumber == 7)
            {
                week8.WeekNumber = (weekFromDb.WeekNumber += 1);
                week8.UserProfileId = weekFromDb.UserProfile.Id;
                week8.AverageWeight = AverageCheckinWeight(weekFromDb.CheckIn1, weekFromDb.CheckIn2, weekFromDb.CheckIn3);
                week8.WeeklyLoss = WeeklyLoss(weekFromDb.AverageWeight, week8.AverageWeight, weekFromDb.WeeklyLoss);
                week8.ExpectedWeight = ExpectedLoss(weekFromDb.ExpectedWeight, weekFromDb.UserProfile.StartWeight, weekFromDb.UserProfile.GoalWeight, weekFromDb.UserProfile.Duration);
                week8.CurrentCalories = weekFromDb.WeeklyCalories;
                week8.WeeklyCalories = WeeklyCal(weekFromDb.ExpectedWeight, weekFromDb.AverageWeight, weekFromDb.WeeklyCalories);
            }
            return View(week8);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Week8(Week week8)
        {
            //Add inputed userdata to Db and Save
            _context.Weeks.Add(week8);
            _context.SaveChanges();

            //check if duration of diet is 8 weeks, if so goto summary else continue
            if (week8.UserProfile.Duration == 8)
            {
                week8.UserProfile.Duration = 0;
                //Add inputed userdata to Db and Save
                _context.Weeks.Add(week8);
                _context.SaveChanges();
                TempData["Success"] = "Great Job, you have successully completed your 6 week weight loss program";
                //You are at the end of the diet
                return RedirectToAction("Summary");
            }
            TempData["Success"] = "Week 8 Completed Successfully, Only 4 to go, You got this";

            return RedirectToAction("Week9");

        }
        //Week 4 Get
        public IActionResult Week9()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            Week week9 = CreateWeek();
            var weekFromDb = GetFirstOrDefaultWeek(u => u.WeekNumber == 8 && u.UserProfile.ApplicationUserId == claim.Value);
            if (weekFromDb.WeekNumber == 8)
            {
                week9.WeekNumber = (weekFromDb.WeekNumber += 1);
                week9.UserProfileId = weekFromDb.UserProfile.Id;
                week9.AverageWeight = AverageCheckinWeight(weekFromDb.CheckIn1, weekFromDb.CheckIn2, weekFromDb.CheckIn3);
                week9.WeeklyLoss = WeeklyLoss(weekFromDb.AverageWeight, week9.AverageWeight, weekFromDb.WeeklyLoss);
                week9.ExpectedWeight = ExpectedLoss(weekFromDb.ExpectedWeight, weekFromDb.UserProfile.StartWeight, weekFromDb.UserProfile.GoalWeight, weekFromDb.UserProfile.Duration);
                week9.CurrentCalories = weekFromDb.WeeklyCalories;
                week9.WeeklyCalories = WeeklyCal(weekFromDb.ExpectedWeight, weekFromDb.AverageWeight, weekFromDb.WeeklyCalories);
                //If we need to add checkin then they are 0
            }
            return View(week9);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Week9(Week week9)
        {
            //Add inputed userdata to Db and Save
            _context.Weeks.Add(week9);
            TempData["Success"] = "Week 9 Completed Successfully";
            _context.SaveChanges();
            return RedirectToAction("Week10");

        }
        //Week 10 Get
        public IActionResult Week10()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            Week week10 = CreateWeek();
            var weekFromDb = GetFirstOrDefaultWeek(u => u.WeekNumber == 9 && u.UserProfile.ApplicationUserId == claim.Value);
            if (weekFromDb.WeekNumber == 9)
            {
                week10.WeekNumber = (weekFromDb.WeekNumber += 1);
                week10.UserProfileId = weekFromDb.UserProfile.Id;
                week10.AverageWeight = AverageCheckinWeight(weekFromDb.CheckIn1, weekFromDb.CheckIn2, weekFromDb.CheckIn3);
                week10.WeeklyLoss = WeeklyLoss(weekFromDb.AverageWeight, week10.AverageWeight, weekFromDb.WeeklyLoss);
                week10.ExpectedWeight = ExpectedLoss(weekFromDb.ExpectedWeight, weekFromDb.UserProfile.StartWeight, weekFromDb.UserProfile.GoalWeight, weekFromDb.UserProfile.Duration);
                week10.CurrentCalories = weekFromDb.WeeklyCalories;
                week10.WeeklyCalories = WeeklyCal(weekFromDb.ExpectedWeight, weekFromDb.AverageWeight, weekFromDb.WeeklyCalories);
            }
            return View(week10);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Week10(Week week10)
        {
            //Add inputed userdata to Db and Save
            _context.Weeks.Add(week10);
            TempData["Success"] = "Week 10 Completed Successfully";
            _context.SaveChanges();
            return RedirectToAction("Week11");

        }
        //Week 4 Get
        public IActionResult Week11()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            Week week11 = CreateWeek();
            var weekFromDb = GetFirstOrDefaultWeek(u => u.WeekNumber == 10 && u.UserProfile.ApplicationUserId == claim.Value);
            if (weekFromDb.WeekNumber == 10)
            {
                week11.WeekNumber = (weekFromDb.WeekNumber += 1);
                week11.UserProfileId = weekFromDb.UserProfile.Id;
                week11.AverageWeight = AverageCheckinWeight(weekFromDb.CheckIn1, weekFromDb.CheckIn2, weekFromDb.CheckIn3);
                week11.WeeklyLoss = WeeklyLoss(weekFromDb.AverageWeight, week11.AverageWeight, weekFromDb.WeeklyLoss);
                week11.ExpectedWeight = ExpectedLoss(weekFromDb.ExpectedWeight, weekFromDb.UserProfile.StartWeight, weekFromDb.UserProfile.GoalWeight, weekFromDb.UserProfile.Duration);
                week11.CurrentCalories = weekFromDb.WeeklyCalories;
                week11.WeeklyCalories = WeeklyCal(weekFromDb.ExpectedWeight, weekFromDb.AverageWeight, weekFromDb.WeeklyCalories);
            }
            return View(week11);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Week11(Week week11)
        {
            //Add inputed userdata to Db and Save
            _context.Weeks.Add(week11);
            TempData["Success"] = "Week 11 Completed Successfully, Home Stretch!!!";
            _context.SaveChanges();
            return RedirectToAction("Week12");

        }
        //Week 12 Get
        public IActionResult Week12()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            Week week12 = CreateWeek();
            var weekFromDb = GetFirstOrDefaultWeek(u => u.WeekNumber == 11 && u.UserProfile.ApplicationUserId == claim.Value);
            if (weekFromDb.WeekNumber == 11)
            {
                week12.WeekNumber = (weekFromDb.WeekNumber += 1);
                week12.UserProfileId = weekFromDb.UserProfile.Id;
                week12.AverageWeight = AverageCheckinWeight(weekFromDb.CheckIn1, weekFromDb.CheckIn2, weekFromDb.CheckIn3);
                week12.WeeklyLoss = WeeklyLoss(weekFromDb.AverageWeight, week12.AverageWeight, weekFromDb.WeeklyLoss);
                week12.ExpectedWeight = ExpectedLoss(weekFromDb.ExpectedWeight, weekFromDb.UserProfile.StartWeight, weekFromDb.UserProfile.GoalWeight, weekFromDb.UserProfile.Duration);
                week12.CurrentCalories = weekFromDb.WeeklyCalories;
                week12.WeeklyCalories = WeeklyCal(weekFromDb.ExpectedWeight, weekFromDb.AverageWeight, weekFromDb.WeeklyCalories);
            }
            return View(week12);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Week12(Week week12)
        {

            //Add inputed userdata to Db and Save
            week12.UserProfile.Duration = 0; //Duration is 0 when Program is complete
            _context.Weeks.Add(week12);
            TempData["Success"] = "Week 12 Completed Successfully, Very well done, YOU DID IT!!!";
            _context.SaveChanges();
            return RedirectToAction("Summary"); // This is the end, Should return summary page with a table with all the stats

        }




        //Create new week instance with current user Id
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
            currentLoss += total;

            return Math.Round(currentLoss, 2);
        }

        //Calculate Weekly expected weight loss
        double ExpectedLoss( double current, double startWeight, double goalWeight, double time)
        {
            var totalLoss = startWeight - goalWeight;
            var weeklyLoss = totalLoss / (time - 1);
            var expectedLoss = current - weeklyLoss;
            return Math.Round(expectedLoss,2);

        }

        //calculate weekly calories
        double WeeklyCal(double expectedWeight, double averageWeight, double calories)
        {
            //if averge weight <= 2% of your expected weight
            if((averageWeight - expectedWeight) >= (expectedWeight * 0.02))
            {
                //subtract calories by 10%
                var weeklycalories = (calories - (calories * 0.1));
                return weeklycalories;
            }
            return Math.Round(calories,2);
            //else do nothing
        }

        //calculate average weight
        double AverageCheckinWeight(double checkIn1, double checkIn2, double checkIn3)
        {
            double avg = ((checkIn1 + checkIn2 + checkIn3) / 3);
            return Math.Round(avg,2);
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



        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            //TODO: Get summary for just the current profile, using profile ID
            IEnumerable<Week> weeks;
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            weeks = GetAllWeeks(u => u.UserProfile.ApplicationUserId == claim.Value);

            return Json(new { data = weeks });

        }
        #endregion
    }


}
