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
            WeekProfile(week);
            //Get Week from db
            var weekFromDb = GetWeekFromDb(week);

            //same as if week == 1 besides incrementing
            if (weekFromDb.WeekNumber == 1)
            {
                week.UserProfileId = weekFromDb.UserProfile.Id;
                week.AverageWeight = AverageCheckinWeight(weekFromDb.CheckIn1, weekFromDb.CheckIn2, weekFromDb.CheckIn3);
                week.WeeklyLoss = WeeklyLoss(weekFromDb.UserProfile.StartWeight, week.AverageWeight, 0); //still 0
                week.ExpectedWeight = ExpectedLoss(weekFromDb.UserProfile.StartWeight, weekFromDb.UserProfile.StartWeight, weekFromDb.UserProfile.GoalWeight, weekFromDb.UserProfile.Duration);
                week.CurrentCalories = weekFromDb.WeeklyCalories;
                week.WeeklyCalories = WeeklyCal(weekFromDb.ExpectedWeight, weekFromDb.AverageWeight, weekFromDb.WeeklyCalories);
                week.CheckIn1 = weekFromDb.CheckIn1;
                week.CheckIn2 = weekFromDb.CheckIn2;
                week.CheckIn3 = weekFromDb.CheckIn3;
                week.WeekNumber = weekFromDb.WeekNumber;
                return View(week);
            }
            if (weekFromDb.WeekNumber == 0)
            {
                week.UserProfileId = week.UserProfile.Id;
                week.WeeklyLoss = WeeklyLoss(week.UserProfile.StartWeight, week.UserProfile.StartWeight, 0); 
                week.ExpectedWeight = week.UserProfile.StartWeight;
                week.AverageWeight = week.UserProfile.StartWeight;
                week.CurrentCalories = week.UserProfile.StartCalories;
                //Start Calories - 10% of start calories
                week.WeeklyCalories = week.UserProfile.StartCalories - (week.UserProfile.StartCalories * 0.1);
                week.CheckIn1 = 0.0;
                week.CheckIn2 = 0.0;
                week.CheckIn3 = 0.0;
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

            WeekProfile(week);
            //Get max week in db
            var weekFromDb = GetWeekFromDb(week);

            //check if week2 has been added to db
            if (weekFromDb.WeekNumber == 0)
            {
                _context.Weeks.Add(week);
                _context.SaveChanges();
                AllCheckedIn(week);
            }

            AllCheckedIn(week);

            //Check if all3 checkins are complete before moving on to the next week
            if (week.CheckIn1 != 0.0 && week.CheckIn2 != 0.0 && week.CheckIn3 != 0.0)
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
            Week week = CreateWeek();
            WeekProfile(week);
            //Get Week from db
            var weekFromDb = GetWeekFromDb(week);
            //and make sure we pull the correct week record
            if (weekFromDb.WeekNumber == 2)
            {
                NewUpdated(week, weekFromDb);
            }
            if (weekFromDb.WeekNumber == 1)
            {
                NewWeek(week, weekFromDb);
            }
            return View(week);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Week2(Week week)
        {
            WeekProfile(week);
            //Get max week in db
            var weekFromDb = GetWeekFromDb(week);

            //check if week2 has been added to db
            if (weekFromDb.WeekNumber == 1)
            {
                _context.Weeks.Add(week);
                _context.SaveChanges();
                AllCheckedIn(week);
            }

            AllCheckedIn(week);

            //Check if all3 checkins are complete before moving on to the next week
            if (week.CheckIn1 != 0.0 && week.CheckIn2 != 0.0 && week.CheckIn3 != 0.0)
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

            Week week3 = CreateWeek();
            WeekProfile(week3);
            //Get Week from db
            var weekFromDb = GetWeekFromDb(week3);
            //and make sure we pull the correct week record
            if (weekFromDb.WeekNumber == 3)
            {
                NewUpdated(week3, weekFromDb);
            }
            if(weekFromDb.WeekNumber == 2)
            {
                NewWeek(week3, weekFromDb);
            }
            return View(week3);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Week3(Week week)
        {

            WeekProfile(week);
            //Get max week in db
            var weekFromDb = GetWeekFromDb(week);

            //check if week2 has been added to db
            if (weekFromDb.WeekNumber == 2)
            {
                _context.Weeks.Add(week);
                _context.SaveChanges();
                AllCheckedIn(week);
            }

            AllCheckedIn(week);

            //Check if all3 checkins are complete before moving on to the next week
            if (week.CheckIn1 != 0.0 && week.CheckIn2 != 0.0 && week.CheckIn3 != 0.0)
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
            Week week = CreateWeek();
            WeekProfile(week);
            //Get Week from db
            var weekFromDb = GetWeekFromDb(week);
            //and make sure we pull the correct week record
            if (weekFromDb.WeekNumber == 4)
            {
                NewUpdated(week, weekFromDb);
            }
            if (weekFromDb.WeekNumber == 3)
            {
                NewWeek(week, weekFromDb);
            }
            return View(week);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Week4(Week week)
        {
            WeekProfile(week);
            //Get max week in db
            var weekFromDb = GetWeekFromDb(week);

            //check if week2 has been added to db
            if (weekFromDb.WeekNumber == 3)
            {
                _context.Weeks.Add(week);
                _context.SaveChanges();
                AllCheckedIn(week);
            }

            AllCheckedIn(week);

            //Check if all3 checkins are complete before moving on to the next week
            if (week.CheckIn1 != 0.0 && week.CheckIn2 != 0.0 && week.CheckIn3 != 0.0)
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
            Week week = CreateWeek();
            WeekProfile(week);
            //Get Week from db
            var weekFromDb = GetWeekFromDb(week);
            //and make sure we pull the correct week record
            if (weekFromDb.WeekNumber == 5)
            {
                NewUpdated(week, weekFromDb);
            }
            if (weekFromDb.WeekNumber == 4)
            {
                NewWeek(week, weekFromDb);
            }
            return View(week);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Week5(Week week)
        {
            WeekProfile(week);
            //Get max week in db
            var weekFromDb = GetWeekFromDb(week);
            //check if week2 has been added to db
            if (weekFromDb.WeekNumber == 4)
            {
                _context.Weeks.Add(week);
                _context.SaveChanges();
                AllCheckedIn(week);
            }

            AllCheckedIn(week);

            //Check if all3 checkins are complete before moving on to the next week
            if (week.CheckIn1 != 0.0 && week.CheckIn2 != 0.0 && week.CheckIn3 != 0.0)
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
            Week week = CreateWeek();
            WeekProfile(week);
            //Get Week from db
            var weekFromDb = GetWeekFromDb(week);
            //and make sure we pull the correct week record
            if (weekFromDb.WeekNumber == 6)
            {
                NewUpdated(week, weekFromDb);
            }
            if (weekFromDb.WeekNumber == 5)
            {
                NewWeek(week, weekFromDb);
            }
            return View(week);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Week6(Week week)
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var weekFromDb = GetWeekFromDb(week);

            // Get profile from Db for user where Duration is not 0 -> where profile is active
            UserProfile userProfile = GetFirstOrDefaultProfile(u => u.ApplicationUserId == claim.Value && u.Duration != 0);

            if (weekFromDb.WeekNumber == 5)
            {
                _context.Weeks.Add(week);
                _context.SaveChanges();
                AllCheckedIn(week);
            }

            AllCheckedIn(week);

            //Check if all3 checkins are complete before moving on to the next week
            if (week.CheckIn1 != 0.0 && week.CheckIn2 != 0.0 && week.CheckIn3 != 0.0)
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
            Week week = CreateWeek();
            WeekProfile(week);
            //Get Week from db
            var weekFromDb = GetWeekFromDb(week);
            //and make sure we pull the correct week record
            if (weekFromDb.WeekNumber == 7)
            {
                NewUpdated(week, weekFromDb);
            }
            if (weekFromDb.WeekNumber == 6)
            {
                NewWeek(week, weekFromDb);
            }
            return View(week);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Week7(Week week)
        {
            WeekProfile(week);
            //Get max week in db
            var weekFromDb = GetWeekFromDb(week);
            //check if week2 has been added to db
            if (weekFromDb.WeekNumber == 6)
            {
                _context.Weeks.Add(week);
                _context.SaveChanges();
                AllCheckedIn(week);
            }

            AllCheckedIn(week);

            //Check if all3 checkins are complete before moving on to the next week
            if (week.CheckIn1 != 0.0 && week.CheckIn2 != 0.0 && week.CheckIn3 != 0.0)
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
            Week week = CreateWeek();
            WeekProfile(week);
            //Get Week from db
            var weekFromDb = GetWeekFromDb(week);
            //and make sure we pull the correct week record
            if (weekFromDb.WeekNumber == 8)
            {
                NewUpdated(week, weekFromDb);
            }
            if (weekFromDb.WeekNumber == 7)
            {
                NewWeek(week, weekFromDb);
            }
            return View(week);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Week8(Week week)
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var weekFromDb = GetWeekFromDb(week);

            // Get profile from Db for user where Duration is not 0 -> where profile is active
            UserProfile userProfile = GetFirstOrDefaultProfile(u => u.ApplicationUserId == claim.Value && u.Duration != 0);

            if (weekFromDb.WeekNumber == 7)
            {
                _context.Weeks.Add(week);
                _context.SaveChanges();
                AllCheckedIn(week);
            }

            AllCheckedIn(week);

            //Check if all3 checkins are complete before moving on to the next week
            if (week.CheckIn1 != 0.0 && week.CheckIn2 != 0.0 && week.CheckIn3 != 0.0)
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
            Week week = CreateWeek();
            WeekProfile(week);
            //Get Week from db
            var weekFromDb = GetWeekFromDb(week);
            //and make sure we pull the correct week record
            if (weekFromDb.WeekNumber == 9)
            {
                NewUpdated(week, weekFromDb);
            }
            if (weekFromDb.WeekNumber == 8)
            {
                NewWeek(week, weekFromDb);
            }
            return View(week);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Week9(Week week)
        {
            WeekProfile(week);
            //Get max week in db
            var weekFromDb = GetWeekFromDb(week);
            //check if week2 has been added to db
            if (weekFromDb.WeekNumber == 8)
            {
                _context.Weeks.Add(week);
                _context.SaveChanges();
                AllCheckedIn(week);
            }

            AllCheckedIn(week);

            //Check if all3 checkins are complete before moving on to the next week
            if (week.CheckIn1 != 0.0 && week.CheckIn2 != 0.0 && week.CheckIn3 != 0.0)
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
            Week week = CreateWeek();
            WeekProfile(week);
            //Get Week from db
            var weekFromDb = GetWeekFromDb(week);
            //and make sure we pull the correct week record
            if (weekFromDb.WeekNumber == 10)
            {
                NewUpdated(week, weekFromDb);
            }
            if (weekFromDb.WeekNumber == 9)
            {
                NewWeek(week, weekFromDb);
            }
            return View(week);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Week10(Week week)
        {
            WeekProfile(week);
            //Get max week in db
            var weekFromDb = GetWeekFromDb(week);
            //check if week2 has been added to db
            if (weekFromDb.WeekNumber == 9)
            {
                _context.Weeks.Add(week);
                _context.SaveChanges();
                AllCheckedIn(week);
            }

            AllCheckedIn(week);

            //Check if all3 checkins are complete before moving on to the next week
            if (week.CheckIn1 != 0.0 && week.CheckIn2 != 0.0 && week.CheckIn3 != 0.0)
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
            Week week = CreateWeek();
            WeekProfile(week);
            //Get Week from db
            var weekFromDb = GetWeekFromDb(week);
            //and make sure we pull the correct week record
            if (weekFromDb.WeekNumber == 11)
            {
                NewUpdated(week, weekFromDb);
            }
            if (weekFromDb.WeekNumber == 10)
            {
                NewWeek(week, weekFromDb);
            }
            return View(week);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Week11(Week week)
        {
            WeekProfile(week);
            //Get max week in db
            var weekFromDb = GetWeekFromDb(week);
            //check if week2 has been added to db
            if (weekFromDb.WeekNumber == 10)
            {
                _context.Weeks.Add(week);
                _context.SaveChanges();
                AllCheckedIn(week);
            }

            AllCheckedIn(week);

            //Check if all3 checkins are complete before moving on to the next week
            if (week.CheckIn1 != 0.0 && week.CheckIn2 != 0.0 && week.CheckIn3 != 0.0)
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
            Week week = CreateWeek();
            WeekProfile(week);
            //Get Week from db
            var weekFromDb = GetWeekFromDb(week);
            //and make sure we pull the correct week record
            if (weekFromDb.WeekNumber == 12)
            {
                NewUpdated(week, weekFromDb);
            }
            if (weekFromDb.WeekNumber == 11)
            {
                NewWeek(week, weekFromDb);
            }
            return View(week);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Week12(Week week)
        {


            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var weekFromDb = GetWeekFromDb(week);

            // Get profile from Db for user where Duration is not 0 -> where profile is active
            UserProfile userProfile = GetFirstOrDefaultProfile(u => u.ApplicationUserId == claim.Value && u.Duration != 0);

            if (weekFromDb.WeekNumber == 11)
            {
                _context.Weeks.Add(week);
                _context.SaveChanges();
                AllCheckedIn(week);
            }

            AllCheckedIn(week);

            //Check if all3 checkins are complete before moving on to the next week
            if (week.CheckIn1 != 0.0 && week.CheckIn2 != 0.0 && week.CheckIn3 != 0.0)
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

        //Function to check if Checkins were entered correctly
        //Also allows for one check in to be entered at a time
        public IActionResult AllCheckedIn(Week week)
        {

            //Get max week in db
            var weekFromDb = GetWeekFromDb(week);

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
                _context.Weeks.Update(weekFromDb);
                _context.SaveChanges();
                return RedirectToWeek(week.WeekNumber);

            }
            //check in 1 and 2 but not 3
            if (week.CheckIn1 != 0.0 && week.CheckIn2 != 0.0 && week.CheckIn3 == 0.0)
            {
                TempData["Success"] = "Check 2 Complted Successfully";
                _context.Weeks.Update(weekFromDb);
                _context.SaveChanges();
                return RedirectToWeek(week.WeekNumber);

            }
            //all check ins filled
            if (week.CheckIn1 != 0.0 && week.CheckIn2 != 0.0 && week.CheckIn3 != 0.0)
            {
                _context.Weeks.Update(weekFromDb);
                _context.SaveChanges();
                return RedirectToWeek(week.WeekNumber+1);

            }
            //If we get here something went wrong
            TempData["Error"] = "Something went wrong, please contact website Admin";
            return RedirectToWeek(week.WeekNumber);
        }
        //Function to update new week instance with values from db
        public Week NewWeek(Week week, Week weekFromDb)
        {
            week.UserProfileId = weekFromDb.UserProfile.Id;
            week.AverageWeight = AverageCheckinWeight(weekFromDb.CheckIn1, weekFromDb.CheckIn2, weekFromDb.CheckIn3);
            week.WeeklyLoss = WeeklyLoss(weekFromDb.AverageWeight, week.AverageWeight, weekFromDb.WeeklyLoss);
            week.ExpectedWeight = ExpectedLoss(weekFromDb.AverageWeight, weekFromDb.UserProfile.StartWeight, weekFromDb.UserProfile.GoalWeight, weekFromDb.UserProfile.Duration);
            week.CurrentCalories = weekFromDb.WeeklyCalories;
            week.WeeklyCalories = WeeklyCal(weekFromDb.ExpectedWeight, weekFromDb.AverageWeight, weekFromDb.WeeklyCalories);
            week.CheckIn1 = 0.0;
            week.CheckIn2 = 0.0;
            week.CheckIn3 = 0.0;
            week.WeekNumber = weekFromDb.WeekNumber += 1;
            return week;
        }
        //Function to update week instance with values from db if new week has already been created
        public Week NewUpdated(Week week, Week weekFromDb)
        {
            week.UserProfileId = weekFromDb.UserProfile.Id;
            week.AverageWeight = AverageCheckinWeight(weekFromDb.CheckIn1, weekFromDb.CheckIn2, weekFromDb.CheckIn3);
            week.WeeklyLoss = WeeklyLoss(weekFromDb.AverageWeight, week.AverageWeight, weekFromDb.WeeklyLoss);
            week.ExpectedWeight = ExpectedLoss(weekFromDb.AverageWeight, weekFromDb.UserProfile.StartWeight, weekFromDb.UserProfile.GoalWeight, weekFromDb.UserProfile.Duration);
            week.CurrentCalories = weekFromDb.WeeklyCalories;
            week.WeeklyCalories = WeeklyCal(weekFromDb.ExpectedWeight, weekFromDb.AverageWeight, weekFromDb.WeeklyCalories);
            week.CheckIn1 = weekFromDb.CheckIn1;
            week.CheckIn2 = weekFromDb.CheckIn2;
            week.CheckIn3 = weekFromDb.CheckIn3;
            week.WeekNumber = weekFromDb.WeekNumber;
            return week;
        }
        //Funtion to get current week from db
        public Week GetWeekFromDb(Week week)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var MaxWeek = _context.Weeks
                .Where(u => u.UserProfile.ApplicationUserId == claim.Value && u.UserProfile.Duration != 0)
                .Select(u => u.WeekNumber)
                .ToList();
            //If there are no weeks in db, return week
            if(MaxWeek.Count == 0)
            {
                return week;
            }
            else
            {
                // we need to pull week from db
                var weekFromDb = GetFirstOrDefaultWeek(u => u.WeekNumber == MaxWeek.Max() && u.UserProfile.ApplicationUserId == claim.Value && u.UserProfile.Id == week.UserProfile.Id);
                return weekFromDb;
            }

        }

        public Week WeekProfile(Week week)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            //we need to check here if there are more than one profile
            //if so, go to the latest profile :) 
            var profileFromDb = GetAllProfiles(u => u.ApplicationUserId == claim.Value && u.Duration != 0, includeProperties: "ApplicationUser");
            //Check if there are multiple UserProfile Id's for this User
            if (profileFromDb != null)
            {
                if (profileFromDb.Count() >= 1)
                {
                    //If yes, get the largest id in the UserProfileId row -- Can turn this into a function called GetProfileId --
                    var profileIds = _context.UserProfiles
                        .Where(u => u.ApplicationUserId == claim.Value)
                        .Select(u => u.Id)
                        .ToList();
                    week.UserProfile = GetFirstOrDefaultProfile(u => u.Id == profileIds.Max());
                }
            }
            return week;

        }
        //Function to end program at any time

        public IActionResult EndProgram()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            // Get profile from Db for user where Duration is not 0 -> where profile is active
            UserProfile userProfile = GetFirstOrDefaultProfile(u => u.ApplicationUserId == claim.Value && u.Duration != 0);

            userProfile.Duration = 0;
            _context.UserProfiles.Update(userProfile);
            TempData["Success"] = "Weight Loss program Ended";
            _context.SaveChanges();
            return RedirectToAction("Summary");
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

        //calculate average weight -- TODO update for instances where only 1 or 2 values are inputted --
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

            weeks = GetAllWeeks(u => u.UserProfile.ApplicationUserId == claim.Value);

            return Json(new { data = weeks });

        }
        #endregion
    }


}
