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
        public Week CreateWeek()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            Week week = new()
            {
                UserProfile = GetFirstOrDefaultProfile(u => u.ApplicationUserId == claim.Value, includeProperties: "ApplicationUser"),
            };
            //week.UserProfileId = GetFirstOrDefaultProfile(u => u.Id == week.UserProfileId);

            return week;
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
                week.ExpectedWeight = week.UserProfile.StartWeight;
                week.AverageWeight = week.UserProfile.StartWeight;
                week.CurrentCalories = week.UserProfile.StartCalories;
                //Start Calories - 10% of start calories
                week.WeeklyCalories = week.UserProfile.StartCalories - (week.UserProfile.StartCalories * 0.1);
                //If we need to add checkin then they are 0
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

            Week week2 = CreateWeek();

            //This is week2
            // we need to pull week 1 from db
            //and make sure we pull the week 1 record
            if (week2.WeekNumber == 1)
            {
                week2.UserProfileId = week2.UserProfile.Id;
                week2.WeekNumber = 2;
                week2.AverageWeight = AverageCheckinWeight(week2.CheckIn1,week2.CheckIn2,week2.CheckIn3);
                week2.ExpectedWeight = AveExpectedWeight(week2.UserProfile.StartWeight, week2.UserProfile.GoalWeight, week2.UserProfile.Duration, week2.AverageWeight);
                week2.CurrentCalories = week2.WeeklyCalories;
                week2.WeeklyCalories = WeeklyCal(week2.ExpectedWeight, week2.AverageWeight, week2.CurrentCalories);
                //If we need to add checkin then they are 0
            }
            return View(week2);
        }


        double AveExpectedWeight( double startWeight, double goalWeight, double time, double currentWeight)
        {
            var totalLoss = goalWeight - startWeight;
            var weeklyLoss = totalLoss / time;
            var averageExpWeight = currentWeight - weeklyLoss;
            return averageExpWeight;
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
