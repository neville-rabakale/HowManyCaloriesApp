﻿using HowManyCalories.Data;
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
        private readonly DataServices services;

        public StartController(ApplicationDbContext context, DataServices services)
        {
            _context = context;
            this.services = services;
        }
        public IActionResult Index()
        {
            return View(new UserProfile());
        }
            

        [Authorize]
        public IActionResult Continue(UserProfile uProfile)
        {
            Week week = new();

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var profileFromDb = services.GetAllProfiles(u => u.ApplicationUserId == claim.Value, includeProperties: "ApplicationUser");
            //Check if there are multiple UserProfile Id's for this User
            if(profileFromDb != null)
            {
                if(profileFromDb.Count() >= 1)
                {
                    //If yes, get the largest id in the UserProfileId row -- Can turn this into a function called GetProfileId --

                    uProfile = services.GetUserProfile(uProfile, claim);

                    //We need to check if the profile is not yet complete -> Duration = 0
                    if ( uProfile.Duration == 0)
                    {
                        TempData["success"] = "You have already completed your weight loss program";
                        //If complete, go to summary
                        return RedirectToAction("Summary","Weeks");
                    }

                    //First we need to check if week exists in the db
                    var weekfromDbExists = services.GetFirstOrDefaultWeek(u => u.UserProfile.ApplicationUserId == claim.Value && u.UserProfile.Duration != 0);
                    // If not return week 1 with profile data
                    if(weekfromDbExists == null)
                    {
                        //continue to first Week
                        return RedirectToWeek(1);

                    }

                    //Before we move forward we need to retrieve the weeks of this profile and 
                    //Return/Goto that particular week
                    var weekFromDb = _context.Weeks
                       .Where(u => u.UserProfile.ApplicationUserId == claim.Value && u.UserProfile.Duration != 0)
                       .Select(u => u.WeekNumber)
                       .ToList();


                    week = services.GetFirstOrDefaultWeek(u => u.UserProfile.ApplicationUserId == claim.Value && u.WeekNumber == weekFromDb.Max() && u.UserProfile.Id == weekfromDbExists.UserProfile.Id);

                    if (week.CheckIn1 != 0.0 && week.CheckIn2 != 0.0 && week.CheckIn3 != 0.0 && week.CheckIn4 != 0.0 && week.CheckIn5 != 0.0)
                    {
                        //continue next Week
                        return RedirectToWeek(week.WeekNumber + 1);
                    }
                    else
                    {
                        //Stay in current Week until completion
                        return RedirectToWeek(week.WeekNumber);
                    }

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
            Week week = new();
            profile.ApplicationUserId = claim.Value;

            var profileFromDb = services.GetAllProfiles(u => u.ApplicationUserId == claim.Value, includeProperties: "ApplicationUser");
            //Check if there are multiple UserProfile Id's for this User
            if (profileFromDb != null)
            {
                if (profileFromDb.Count() >= 1)
                {
                    //If yes, get the largest id in the UserProfileId row -- Can turn this into a function called GetProfileId --
                    var uProfile = services.GetUserProfile(profile, claim);
                    if (uProfile.Duration != 0)
                    {
                        //Before we move forward we need to retrieve the weeks of this profile and 
                        //Return/Goto that particular week
                        var weekFromDb = _context.Weeks
                           .Where(u => u.UserProfile.ApplicationUserId == claim.Value)
                           .Select(u => u.WeekNumber)
                           .ToList();

                        //First we need to check if week exists in the db
                        var weekfromDbExists = services.GetFirstOrDefaultWeek(u => u.UserProfile.ApplicationUserId == claim.Value);
                        // If not return week 1 with profile data
                        if (weekfromDbExists == null)
                        {
                            TempData["error"] = "Please complete current weight loss program before starting a new one";
                            return RedirectToWeek(week.WeekNumber + 1);
                        }

                        // need to get the week for the correct profileId
                        week = services.GetFirstOrDefaultWeek(u => u.UserProfile.ApplicationUserId == claim.Value && u.WeekNumber == weekFromDb.Max() && u.UserProfile.Id == uProfile.Id);
                        
                        TempData["error"] = "Please complete current weight loss program before starting a new one";
                        return RedirectToWeek((week.WeekNumber + 1));
                    }
                }
            }
            //We need to check that the target is < the start weight
            if(profile.GoalWeight > profile.StartWeight)
            {
                TempData["error"] = "Your  Goal weight needs to be less than your start weight, Please chech that you have entered correcly";
                return RedirectToAction("Index");
            }
            //Before we start, we need to check if the goal is realisitc
            //Check if the weight loss is less than 1% body weight per week
            var weeklyLoss = (profile.StartWeight - profile.GoalWeight) / (profile.Duration);
            if(weeklyLoss > 1 || weeklyLoss < 0.5)
            {
                TempData["error"] = "For optimum weight loss, your weight loss goal should be losing between 0.5% and 1% of your body weight per week, " +
                    "Please adjust your duration or your goal weight accordingly. Your current weekly goal is" + Math.Round(weeklyLoss,2).ToString();

                return RedirectToAction("Index");
            }
            //Add inputed userdata to Db and Save
            _context.UserProfiles.Add(profile);
            TempData["Success"] = "Start data created Successfully";
            _context.SaveChanges();
            return RedirectToAction("Week1", "Weeks");

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
