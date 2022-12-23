using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Linq;
using System.Security.Claims;
using HowManyCalories.Data;

namespace HowManyCalories.Models
{
    public class DataServices
    {
        private readonly ApplicationDbContext context;
        private readonly ClaimsPrincipal user;

        public DataServices(ApplicationDbContext context, ClaimsPrincipal user)
        {
            this.context = context;
            this.user = user;
        }


        //fuction to get profile using claimsId, need to update -- Unsused so far
        public UserProfile GetUserProfile(UserProfile profile, Claim claim)
        {
            var profileIds = context.UserProfiles
                .Where(u => u.ApplicationUserId == claim.Value)
                .Select(u => u.Id)
                .ToList();
            return profile = GetFirstOrDefaultProfile(u => u.Id == profileIds.Max());
        }


        //GetAll Query for UserProfiles
        public IEnumerable<UserProfile> GetAllProfiles(Expression<Func<UserProfile, bool>>? filter = null, string? includeProperties = null)
        {
            //First we need to query the db
            IQueryable<UserProfile> query = context.UserProfiles;
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
            IQueryable<UserProfile> query = context.UserProfiles;
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
            IQueryable<Week> query = context.Weeks;
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


        //Below are the weeks servives
        public Week NewWeek(Week week, Week weekFromDb)
        {
            week.UserProfileId = weekFromDb.UserProfile.Id;
            week.AverageWeight = AverageCheckinWeight(weekFromDb.CheckIn1, weekFromDb.CheckIn2, weekFromDb.CheckIn3, weekFromDb.CheckIn4, weekFromDb.CheckIn5);
            week.WeeklyLoss = WeeklyLoss(weekFromDb.AverageWeight, week.AverageWeight, weekFromDb);
            week.ExpectedWeight = ExpectedLoss(weekFromDb.UserProfile.StartWeight, weekFromDb.UserProfile.GoalWeight, weekFromDb.UserProfile.Duration, (weekFromDb.WeekNumber + 1));
            week.CurrentCalories = weekFromDb.WeeklyCalories;
            week.WeeklyCalories = weekFromDb.WeeklyCalories;
            week.CheckIn1 = 0.0;
            week.CheckIn2 = 0.0;
            week.CheckIn3 = 0.0;
            week.CheckIn4 = 0.0;
            week.CheckIn5 = 0.0;
            week.WeekNumber = weekFromDb.WeekNumber += 1;
            return week;
        }
        //Function to update week instance with values from db if new week has already been created
        public Week NewUpdated(Week week, Week weekFromDb)
        {
            week.UserProfileId = weekFromDb.UserProfile.Id;
            week.CheckIn1 = weekFromDb.CheckIn1;
            week.CheckIn2 = weekFromDb.CheckIn2;
            week.CheckIn3 = weekFromDb.CheckIn3;
            week.CheckIn4 = weekFromDb.CheckIn4;
            week.CheckIn5 = weekFromDb.CheckIn5;
            week.AverageWeight = AverageCheckinWeight(weekFromDb.CheckIn1, weekFromDb.CheckIn2, weekFromDb.CheckIn3, weekFromDb.CheckIn4, weekFromDb.CheckIn5);
            week.WeeklyLoss = WeeklyLoss(weekFromDb.AverageWeight, week.AverageWeight, weekFromDb);
            week.ExpectedWeight = ExpectedLoss(weekFromDb.UserProfile.StartWeight, weekFromDb.UserProfile.GoalWeight, weekFromDb.UserProfile.Duration, weekFromDb.WeekNumber);
            week.CurrentCalories = weekFromDb.WeeklyCalories;
            week.WeeklyCalories = WeeklyCal(weekFromDb.ExpectedWeight, week.AverageWeight, weekFromDb.WeeklyCalories, weekFromDb);
            week.WeekNumber = weekFromDb.WeekNumber;
            return week;
        }
        //Funtion to get current week from db
        public Week GetWeekFromDb(Week week)
        {
            var claimsIdentity = (ClaimsIdentity)user.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var MaxWeek = context.Weeks
                .Where(u => u.UserProfile.ApplicationUserId == claim.Value && u.UserProfile.Duration != 0)
                .Select(u => u.WeekNumber)
                .ToList();
            //If there are no weeks in db, return week
            if (MaxWeek.Count == 0)
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

        //get previous week from db
        public Week GetPrevWeekFromDb(Week week)
        {
            var claimsIdentity = (ClaimsIdentity)user.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var MaxWeek = context.Weeks
                .Where(u => u.UserProfile.ApplicationUserId == claim.Value && u.UserProfile.Duration != 0)
                .Select(u => u.WeekNumber)
                .ToList();
            //If there are no weeks in db, return week
            if (MaxWeek.Count == 0)
            {
                return week;
            }
            if (MaxWeek.Count == 1)
            {
                var weekFromDb = GetFirstOrDefaultWeek(u => u.WeekNumber == (MaxWeek.Max()) && u.UserProfile.ApplicationUserId == claim.Value && u.UserProfile.Id == week.UserProfile.Id);
                return weekFromDb;

            }
            else
            {
                // we need to pull week from db
                var weekFromDb = GetFirstOrDefaultWeek(u => u.WeekNumber == (MaxWeek.Max() - 1) && u.UserProfile.ApplicationUserId == claim.Value && u.UserProfile.Id == week.UserProfile.Id);

                return weekFromDb;
            }

        }

        public Week WeekProfile(Week week)
        {
            var claimsIdentity = (ClaimsIdentity)user.Identity;
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
                    var profileIds = context.UserProfiles
                        .Where(u => u.ApplicationUserId == claim.Value)
                        .Select(u => u.Id)
                        .ToList();
                    week.UserProfile = GetFirstOrDefaultProfile(u => u.Id == profileIds.Max());
                }
            }
            return week;

        }
        //Function to end program at any time

        //Create new week instance with current user Id
        public Week CreateWeek()
        {
            var claimsIdentity = (ClaimsIdentity)user.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            Week week = new()
            {
                UserProfile = GetFirstOrDefaultProfile(u => u.ApplicationUserId == claim.Value, includeProperties: "ApplicationUser"),
            };
            return week;
        }

        //How much did you actually lose in total
        public double WeeklyLoss(double previous, double current, Week week)
        {
            double currentLoss;
            var weekFromDb = GetPrevWeekFromDb(week);
            var total = weekFromDb.WeeklyLoss;
            //Week 1
            if (weekFromDb.WeekNumber == 0)
            {
                currentLoss = previous - current;
                return Math.Round(currentLoss, 2);
            }

            //The rest of the Weeks
            currentLoss = weekFromDb.AverageWeight - current;
            currentLoss += total;

            return Math.Round(currentLoss, 2);
        }

        //Calculate Weekly expected weight loss
        public double ExpectedLoss(double startWeight, double goalWeight, double time, double theWeek)
        {
            var totalLoss = startWeight - goalWeight;
            var weeklyLoss = totalLoss / time;
            var expectedLoss = startWeight - (weeklyLoss * theWeek);
            return Math.Round(expectedLoss, 2);

        }

        //calculate weekly calories
        public double WeeklyCal(double expectedWeight, double averageWeight, double calories, Week week)
        {

            if (week.CheckIn1 == 0 && week.CheckIn2 == 0 && week.CheckIn3 == 0)
            {
                return Math.Round(calories);
            }
            //if averge weight >= 2% of your expected weight
            if (averageWeight >= (expectedWeight + (expectedWeight * 0.02)))
            {
                //subtract calories by 10%
                var weeklycalories = (calories - (calories * 0.1));
                return Math.Round(weeklycalories);
            }
            //if averge weight >= 2.2% of your expected weight
            if (averageWeight >= (expectedWeight + (expectedWeight * 0.022)))
            {
                //subtract calories by 20%
                var weeklycalories = (calories - (calories * 0.2));
                return Math.Round(weeklycalories);
            }
            //if average weight < 2% of your expected weight
            if (averageWeight < (expectedWeight - (expectedWeight * 0.02)))
            {
                //add calories by 10%
                var weeklycalories = (calories + (calories * 0.1));
                return Math.Round(weeklycalories);
            }
            return Math.Round(calories);
            //else do nothing
        }

        //calculate average weekly weight
        public double AverageCheckinWeight(double checkIn1, double checkIn2, double checkIn3, double checkIn4, double checkIn5)
        {
            double avg;

            if (checkIn1 == 0 && checkIn2 == 0 && checkIn3 == 0 && checkIn4 == 0 && checkIn5 == 0)
            {
                avg = 0.0;
                return Math.Round(avg, 2);
            }
            if (checkIn1 != 0 && checkIn2 == 0 && checkIn3 == 0 && checkIn4 == 0 && checkIn5 == 0)
            {
                avg = checkIn1;
                return Math.Round(avg, 2);
            }
            if (checkIn1 != 0 && checkIn2 != 0 && checkIn3 == 0 && checkIn4 == 0 && checkIn5 == 0)
            {
                avg = ((checkIn1 + checkIn2) / 2);
                return Math.Round(avg, 2);
            }
            if (checkIn1 != 0 && checkIn2 != 0 && checkIn3 != 0 && checkIn4 == 0 && checkIn5 == 0)
            {
                avg = ((checkIn1 + checkIn2 + checkIn3) / 3);
                return Math.Round(avg, 2);
            }
            if (checkIn1 != 0 && checkIn2 != 0 && checkIn3 != 0 && checkIn4 != 0 && checkIn5 == 0)
            {
                avg = ((checkIn1 + checkIn2 + checkIn3 + checkIn4) / 4);
                return Math.Round(avg, 2);
            }
            else
            {
                avg = ((checkIn1 + checkIn2 + checkIn3 + checkIn4 + checkIn5) / 5);
            }
            return Math.Round(avg, 2);

        }

        //Get All Query for Weeks
        public IEnumerable<Week> GetAllWeeks(Expression<Func<Week, bool>>? filter = null, string? includeProperties = null)
        {

            //First we need to query the db
            IQueryable<Week> query = context.Weeks;
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




    }
}
