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
        private readonly DataServices services;

        public WeeksController(ApplicationDbContext context, DataServices services)
        {
            _context = context;
            this.services = services;
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
            Week week = services.CreateWeek();
            services.WeekProfile(week);
            //Get Week from db
            var weekFromDb = services.GetWeekFromDb(week);

            //same as if week == 1 besides incrementing
            if (weekFromDb.WeekNumber == 1)
            {
                week.UserProfileId = weekFromDb.UserProfile.Id;
                week.AverageWeight = services.AverageCheckinWeight(weekFromDb.CheckIn1, weekFromDb.CheckIn2, weekFromDb.CheckIn3, weekFromDb.CheckIn4, weekFromDb.CheckIn5);
                week.WeeklyLoss = services.WeeklyLoss(weekFromDb.UserProfile.StartWeight, week.AverageWeight, weekFromDb);
                week.ExpectedWeight = services.ExpectedLoss(weekFromDb.UserProfile.StartWeight, weekFromDb.UserProfile.GoalWeight, weekFromDb.UserProfile.Duration, weekFromDb.WeekNumber);
                week.CurrentCalories = weekFromDb.WeeklyCalories;
                week.WeeklyCalories = services.WeeklyCal(weekFromDb.ExpectedWeight, weekFromDb.AverageWeight, weekFromDb.WeeklyCalories, weekFromDb);
                week.CheckIn1 = weekFromDb.CheckIn1;
                week.CheckIn2 = weekFromDb.CheckIn2;
                week.CheckIn3 = weekFromDb.CheckIn3;
                week.CheckIn4 = weekFromDb.CheckIn4;
                week.CheckIn5 = weekFromDb.CheckIn5;
                week.WeekNumber = weekFromDb.WeekNumber;
                return View(week);
            }
            if (weekFromDb.WeekNumber == 0)
            {
                week.UserProfileId = week.UserProfile.Id;
                week.WeeklyLoss = services.WeeklyLoss(week.UserProfile.StartWeight, week.UserProfile.StartWeight, weekFromDb); 
                week.ExpectedWeight = week.UserProfile.StartWeight;
                week.AverageWeight = week.UserProfile.StartWeight;
                week.CurrentCalories = week.UserProfile.StartCalories;
                //Start Calories - 10% of start calories
                week.WeeklyCalories = week.UserProfile.StartCalories - (week.UserProfile.StartCalories * 0.1);
                week.CheckIn1 = 0.0;
                week.CheckIn2 = 0.0;
                week.CheckIn3 = 0.0;
                week.CheckIn4 = 0.0;
                week.CheckIn5 = 0.0;
                week.WeekNumber = week.WeekNumber += 1;
                return View(week);
            }


            return View(week);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Week1(Week week)
        {

            services.WeekProfile(week);
            //Get max week in db
            var weekFromDb = services.GetWeekFromDb(week);

            //check if week2 has been added to db
            if (weekFromDb.WeekNumber == 0)
            {
                _context.Weeks.Add(week);
                _context.SaveChanges();
                AllCheckedIn(week);
            }

            AllCheckedIn(week);

            //Check if all5 checkins are complete before moving on to the next week
            if (week.CheckIn1 != 0.0 && week.CheckIn2 != 0.0 && week.CheckIn3 != 0.0 && week.CheckIn4 != 0.0 && week.CheckIn5 != 0.0)
            {
                //continue to Week 5
                TempData["Success"] = "Week 2 Completed Successfully";
                return RedirectToWeek(week.WeekNumber + 1);
            }
            else
            {
                //Stay in Week until completion
                return RedirectToWeek(week.WeekNumber);
            }

        }

        //Week 2 Get
        public IActionResult Week2()
        {
            int weekNum = 2;
            try
            {
                return GetWeekAction(weekNum);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Something went wrong{e}");
            }
            return RedirectToWeek(weekNum);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Week2(Week week)
        {
            services.WeekProfile(week);
            //Get max week in db
            var weekFromDb = services.GetWeekFromDb(week);

            //check if week2 has been added to db
            if (weekFromDb.WeekNumber == 1)
            {
                _context.Weeks.Add(week);
                _context.SaveChanges();
                AllCheckedIn(week);
            }

            AllCheckedIn(week);

            //Check if all5 checkins are complete before moving on to the next week
            if (week.CheckIn1 != 0.0 && week.CheckIn2 != 0.0 && week.CheckIn3 != 0.0 && week.CheckIn4 != 0.0 && week.CheckIn5 != 0.0)
            {
                //continue to Week 5
                TempData["Success"] = "Week 2 Completed Successfully";
                return RedirectToWeek(week.WeekNumber + 1);
            }
            else
            {
                //Stay in Week until completion
                return RedirectToWeek(week.WeekNumber);
            }

        }

        //Week 3 Get
        public IActionResult Week3()
        {
            int weekNum = 3;
            try
            {
                return GetWeekAction(weekNum);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Something went wrong{e}");
            }
            return RedirectToWeek(weekNum);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Week3(Week week)
        {

            services.WeekProfile(week);
            //Get max week in db
            var weekFromDb = services.GetWeekFromDb(week);

            //check if week2 has been added to db
            if (weekFromDb.WeekNumber == 2)
            {
                _context.Weeks.Add(week);
                _context.SaveChanges();
                AllCheckedIn(week);
            }

            AllCheckedIn(week);

            //Check if all5 checkins are complete before moving on to the next week
            if (week.CheckIn1 != 0.0 && week.CheckIn2 != 0.0 && week.CheckIn3 != 0.0 && week.CheckIn4 != 0.0 && week.CheckIn5 != 0.0)
            {
                //continue to Week 4
                TempData["Success"] = "Week 3 Completed Successfully";
                return RedirectToWeek(week.WeekNumber + 1);
            }
            else
            {
                //Stay in Week until completion
                return RedirectToWeek(week.WeekNumber);
            }


        }
        //Week 4 Get
        public IActionResult Week4()
        {
            int weekNum = 4;
            try
            {
                return GetWeekAction(weekNum);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Something went wrong{e}");
            }
            return RedirectToWeek(weekNum);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Week4(Week week)
        {
            services.WeekProfile(week);
            //Get max week in db
            var weekFromDb = services.GetWeekFromDb(week);

            //check if week2 has been added to db
            if (weekFromDb.WeekNumber == 3)
            {
                _context.Weeks.Add(week);
                _context.SaveChanges();
                AllCheckedIn(week);
            }

            AllCheckedIn(week);

            //Check if all5 checkins are complete before moving on to the next week
            if (week.CheckIn1 != 0.0 && week.CheckIn2 != 0.0 && week.CheckIn3 != 0.0 && week.CheckIn4 != 0.0 && week.CheckIn5 != 0.0)
            {
                //continue to Week 5
                TempData["Success"] = "Week 4 Completed Successfully";
                return RedirectToWeek(week.WeekNumber + 1);
            }
            else
            {
                //Stay in Week until completion
                return RedirectToWeek(week.WeekNumber);
            }

        }
        //Week 5 Get
        public IActionResult Week5()
        {
            int weekNum = 5;
            try
            {
                return GetWeekAction(weekNum);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Something went wrong{e}");
            }
            return RedirectToWeek(weekNum);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Week5(Week week)
        {
            services.WeekProfile(week);
            //Get max week in db
            var weekFromDb = services.GetWeekFromDb(week);
            //check if week2 has been added to db
            if (weekFromDb.WeekNumber == 4)
            {
                _context.Weeks.Add(week);
                _context.SaveChanges();
                AllCheckedIn(week);
            }

            AllCheckedIn(week);

            //Check if all5 checkins are complete before moving on to the next week
            if (week.CheckIn1 != 0.0 && week.CheckIn2 != 0.0 && week.CheckIn3 != 0.0 && week.CheckIn4 != 0.0 && week.CheckIn5 != 0.0)
            {
                //continue to Week 5
                TempData["Success"] = "Week 5 Completed Successfully";
                return RedirectToWeek(week.WeekNumber + 1);
            }
            else
            {
                //Stay in Week until completion
                return RedirectToWeek(week.WeekNumber);
            }

        }
        //Week 6 Get
        public IActionResult Week6()
        {
            int weekNum = 6;
            try
            {
                return GetWeekAction(weekNum);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Something went wrong{e}");
            }
            return RedirectToWeek(weekNum);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Week6(Week week)
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var weekFromDb = services.GetWeekFromDb(week);

            // Get profile from Db for user where Duration is not 0 -> where profile is active
            UserProfile userProfile = services.GetFirstOrDefaultProfile(u => u.ApplicationUserId == claim.Value && u.Duration != 0);

            if (weekFromDb.WeekNumber == 5)
            {
                _context.Weeks.Add(week);
                _context.SaveChanges();
                AllCheckedIn(week);
            }

            AllCheckedIn(week);

            //Check if all5 checkins are complete before moving on to the next week
            if (week.CheckIn1 != 0.0 && week.CheckIn2 != 0.0 && week.CheckIn3 != 0.0 && week.CheckIn4 != 0.0 && week.CheckIn5 != 0.0)
            {
                //check if duration of diet is 6 weeks, if so goto summary else continue
                if (userProfile.Duration == 6 && week.WeekNumber == 6)
                {
                    //Set Duration to 0 -> End of program
                    userProfile.Duration = 0;
                    _context.UserProfiles.Update(userProfile);
                    _context.SaveChanges();
                    TempData["Success"] = "Great Job, you have successully completed your 6 week weight loss program";
                    //You are at the end of the diet
                    return RedirectToAction("Summary");
                }
                else
                {
                    TempData["Success"] = "Week 6 Completed Successfully, Keep it going!";
                    return RedirectToWeek(week.WeekNumber + 1);
                }
            }
            else
            {
                //Stay in Week until completion
                return RedirectToWeek(week.WeekNumber);
            }
        }

        //Week 7 Get
        public IActionResult Week7()
        {
            int weekNum = 7;
            try
            {
                return GetWeekAction(weekNum);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Something went wrong{e}");
            }
            return RedirectToWeek(weekNum);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Week7(Week week)
        {
            services.WeekProfile(week);
            //Get max week in db
            var weekFromDb = services.GetWeekFromDb(week);
            //check if week2 has been added to db
            if (weekFromDb.WeekNumber == 6)
            {
                _context.Weeks.Add(week);
                _context.SaveChanges();
                AllCheckedIn(week);
            }

            AllCheckedIn(week);

            //Check if all5 checkins are complete before moving on to the next week
            if (week.CheckIn1 != 0.0 && week.CheckIn2 != 0.0 && week.CheckIn3 != 0.0 && week.CheckIn4 != 0.0 && week.CheckIn5 != 0.0)
            {
                //continue to Week 8
                TempData["Success"] = "Week 7 Completed Successfully";
                return RedirectToWeek(week.WeekNumber + 1);
            }
            else
            {
                //Stay in Week  until completion
                return RedirectToWeek(week.WeekNumber);
            }

        }
        //Week 8 Get
        public IActionResult Week8()
        {
            int weekNum = 8;
            try
            {
                return GetWeekAction(weekNum);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Something went wrong{e}");
            }
            return RedirectToWeek(weekNum);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Week8(Week week)
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var weekFromDb = services.GetWeekFromDb(week);

            // Get profile from Db for user where Duration is not 0 -> where profile is active
            UserProfile userProfile = services.GetFirstOrDefaultProfile(u => u.ApplicationUserId == claim.Value && u.Duration != 0);

            if (weekFromDb.WeekNumber == 7)
            {
                _context.Weeks.Add(week);
                _context.SaveChanges();
                AllCheckedIn(week);
            }

            AllCheckedIn(week);

            //Check if all5 checkins are complete before moving on to the next week
            if (week.CheckIn1 != 0.0 && week.CheckIn2 != 0.0 && week.CheckIn3 != 0.0 && week.CheckIn4 != 0.0 && week.CheckIn5 != 0.0)
            {
                //check if duration of diet is 6 weeks, if so goto summary else continue
                if (userProfile.Duration == 8 && week.WeekNumber == 8)
                {
                    //Set Duration to 0 -> End of program
                    userProfile.Duration = 0;
                    _context.UserProfiles.Update(userProfile);
                    _context.SaveChanges();
                    TempData["Success"] = "Great Job, you have successully completed your 8 week wight loss program, well done!!";
                    //You are at the end of the diet
                    return RedirectToAction("Summary");
                }
                else
                {
                    TempData["Success"] = "Week 8 Completed Successfully, Only 4 to go, You got this";
                    return RedirectToWeek(week.WeekNumber + 1);
                }

            }
            else
            {
                //Stay in Week until completion
                return RedirectToWeek(week.WeekNumber);
            }

        }
        //Week 9 Get
        public IActionResult Week9()
        {
            int weekNum = 9;
            try
            {
                return GetWeekAction(weekNum);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Something went wrong{e}");
            }
            return RedirectToWeek(weekNum);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Week9(Week week)
        {
            services.WeekProfile(week);
            //Get max week in db
            var weekFromDb = services.GetWeekFromDb(week);
            //check if week2 has been added to db
            if (weekFromDb.WeekNumber == 8)
            {
                _context.Weeks.Add(week);
                _context.SaveChanges();
                AllCheckedIn(week);
            }

            AllCheckedIn(week);

            //Check if all5 checkins are complete before moving on to the next week
            if (week.CheckIn1 != 0.0 && week.CheckIn2 != 0.0 && week.CheckIn3 != 0.0 && week.CheckIn4 != 0.0 && week.CheckIn5 != 0.0)
            {
                //continue to Week 10
                TempData["Success"] = "Week 9 Completed Successfully";
                return RedirectToWeek(week.WeekNumber + 1);
            }
            else
            {
                //Stay in Week until completion
                return RedirectToWeek(week.WeekNumber);
            }

        }
        //Week 10 Get
        public IActionResult Week10()
        {
            int weekNum = 10;
            try
            {
                return GetWeekAction(weekNum);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Something went wrong{e}");
            }
            return RedirectToWeek(weekNum);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Week10(Week week)
        {
            services.WeekProfile(week);
            //Get max week in db
            var weekFromDb = services.GetWeekFromDb(week);
            //check if week2 has been added to db
            if (weekFromDb.WeekNumber == 9)
            {
                _context.Weeks.Add(week);
                _context.SaveChanges();
                AllCheckedIn(week);
            }

            AllCheckedIn(week);

            //Check if all5 checkins are complete before moving on to the next week
            if (week.CheckIn1 != 0.0 && week.CheckIn2 != 0.0 && week.CheckIn3 != 0.0 && week.CheckIn4 != 0.0 && week.CheckIn5 != 0.0)
            {
                //continue to Week 11
                TempData["Success"] = "Week 10 Completed Successfully";
                return RedirectToWeek(week.WeekNumber + 1);
            }
            else
            {
                //Stay in Week until completion
                return RedirectToWeek(week.WeekNumber);
            }

        }
        //Week 11 Get
        public IActionResult Week11()
        {
            int weekNum = 11;
            try
            {
                return GetWeekAction(weekNum);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Something went wrong{e}");
            }
            return RedirectToWeek(weekNum);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Week11(Week week)
        {
            services.WeekProfile(week);
            //Get max week in db
            var weekFromDb = services.GetWeekFromDb(week);
            //check if week2 has been added to db
            if (weekFromDb.WeekNumber == 10)
            {
                _context.Weeks.Add(week);
                _context.SaveChanges();
                AllCheckedIn(week);
            }

            AllCheckedIn(week);

            //Check if all5 checkins are complete before moving on to the next week
            if (week.CheckIn1 != 0.0 && week.CheckIn2 != 0.0 && week.CheckIn3 != 0.0 && week.CheckIn4 != 0.0 && week.CheckIn5 != 0.0)
            {
                //continue to Week 12
                TempData["Success"] = "Week 11 Completed Successfully";
                return RedirectToWeek(week.WeekNumber + 1);
            }
            else
            {
                //Stay in Week until completion
                return RedirectToWeek(week.WeekNumber);
            }

        }
        //Week 12 Get
        public IActionResult Week12()
        {
            int weekNum = 12;
            try
            {
                return GetWeekAction(weekNum);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Something went wrong{e}");
            }
            return RedirectToWeek(weekNum);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Week12(Week week)
        {


            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var weekFromDb = services.GetWeekFromDb(week);

            // Get profile from Db for user where Duration is not 0 -> where profile is active
            UserProfile userProfile = services.GetFirstOrDefaultProfile(u => u.ApplicationUserId == claim.Value && u.Duration != 0);

            if (weekFromDb.WeekNumber == 11)
            {
                _context.Weeks.Add(week);
                _context.SaveChanges();
                AllCheckedIn(week);
            }

            AllCheckedIn(week);

            //Check if all5 checkins are complete before moving on to the next week
            if (week.CheckIn1 != 0.0 && week.CheckIn2 != 0.0 && week.CheckIn3 != 0.0 && week.CheckIn4 != 0.0 && week.CheckIn5 != 0.0)
            {
                //Set Duration to 0 -> End of program
                userProfile.Duration = 0;
                _context.UserProfiles.Update(userProfile);
                TempData["Success"] = "Week 12 Completed Successfully, Very well done, YOU DID IT!!!";
                _context.SaveChanges();
                return RedirectToAction("Summary"); // This is the end, return to summary page with a table with all the stats
            }
            else
            {
                //Stay in Week until completion
                return RedirectToWeek(week.WeekNumber);
            }

        }


        //Get week Action that takes current week number as parameter and returns current week action
        public IActionResult GetWeekAction(int currentWeek)
        {
            Week week = services.CreateWeek();
            services.WeekProfile(week);
            //Get Week from db
            var weekFromDb = services.GetWeekFromDb(week);
            //and make sure we pull the correct week record
            if (weekFromDb.WeekNumber == currentWeek)
            {
                services.NewUpdated(week, weekFromDb);
            }
            if (weekFromDb.WeekNumber == currentWeek - 1)
            {
                services.NewWeek(week, weekFromDb);
            }
            return View(week);
        }


        //Function to check if Checkins were entered correctly
        //Also allows for one check in to be entered at a time
        public IActionResult AllCheckedIn(Week week)
        {

            //Get max week in db
            var weekFromDb = services.GetWeekFromDb(week);

            //check in 2 or 3 filled but not check in 1
            if (week.CheckIn1 == 0.0 && (week.CheckIn2 != 0.0 || week.CheckIn3 != 0.0))
            {
                TempData["Error"] = "Please fill in the check in data in order";
                return RedirectToWeek(week.WeekNumber);
            }
            //check in 1 and 3 filled but not check in 2
            if (week.CheckIn1 != 0.0 && week.CheckIn2 == 0.0 & week.CheckIn3 != 0.0)
            {
                TempData["Error"] = "Please fill in the check in data in order";
                return RedirectToWeek(week.WeekNumber);
            }

            //all checkins are not filled in
            if (week.CheckIn1 == 0.0 && week.CheckIn2 == 0.0 && week.CheckIn3 == 0.0)
            {
                TempData["Error"] = "Please make sure that you filled in your check in data for the week";
                return RedirectToWeek(week.WeekNumber);
            }
            //check in 1 but not 2 or 3
            if (week.CheckIn1 != 0.0 && week.CheckIn2 == 0.0 & week.CheckIn3 == 0.0)
            {
                TempData["Success"] = "Check 1 Complted Successfully";
                weekFromDb.CheckIn1 = week.CheckIn1;
                weekFromDb.CheckIn2 = week.CheckIn2;
                weekFromDb.CheckIn3 = week.CheckIn3;
                _context.Weeks.Update(weekFromDb);
                _context.SaveChanges();
                return RedirectToWeek(week.WeekNumber);

            }
            //check in 1 and 2 but not 3
            if (week.CheckIn1 != 0.0 && week.CheckIn2 != 0.0 && week.CheckIn3 == 0.0)
            {
                TempData["Success"] = "Check 2 Complted Successfully";
                weekFromDb.CheckIn1 = week.CheckIn1;
                weekFromDb.CheckIn2 = week.CheckIn2;
                weekFromDb.CheckIn3 = week.CheckIn3;
                _context.Weeks.Update(weekFromDb);
                _context.SaveChanges();
                return RedirectToWeek(week.WeekNumber);

            }
            //all check ins filled
            if (week.CheckIn1 != 0.0 && week.CheckIn2 != 0.0 && week.CheckIn3 != 0.0)
            {
                weekFromDb.CheckIn1 = week.CheckIn1;
                weekFromDb.CheckIn2 = week.CheckIn2;
                weekFromDb.CheckIn3 = week.CheckIn3;
                _context.Weeks.Update(weekFromDb);
                _context.SaveChanges();
                return RedirectToWeek(week.WeekNumber+1);

            }
            //If we get here something went wrong
            TempData["Error"] = "Something went wrong, please contact website Admin";
            return RedirectToWeek(week.WeekNumber);
        }
        public IActionResult EndProgram()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            // Get profile from Db for user where Duration is not 0 -> where profile is active
            UserProfile userProfile = services.GetFirstOrDefaultProfile(u => u.ApplicationUserId == claim.Value && u.Duration != 0);

            userProfile.Duration = 0;
            _context.UserProfiles.Update(userProfile);
            TempData["Success"] = "Weight Loss program Ended";
            _context.SaveChanges();
            return RedirectToAction("Summary");
        }

        //Returns Action to particular weekNumber
        public IActionResult RedirectToWeek(int weekNumber)
        {
            if (weekNumber == 1)
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

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            //TODO: Get summary for just the current profile, using profile ID
            IEnumerable<Week> weeks;
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            weeks = services.GetAllWeeks(u => u.UserProfile.ApplicationUserId == claim.Value);

            return Json(new { data = weeks });

        }
        #endregion
    }


}
