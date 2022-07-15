using HowManyCalories.Data;
using HowManyCalories.Models;
using HowManyCalories.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HowManyCalories.DbInitialazer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _config;



        public DbInitializer(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db,
            IConfiguration config)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
            _config = config;
        }
        public void Initialize()
        {
            var MailPass = _config["MailPass"];
            // Apply migrations if not applied
            try
            {
                //check if there has been any migrations
                if(_db.Database.GetPendingMigrations().Count() > 0)
                {
                    //if there are pending migrations -> migrate
                    _db.Database.Migrate();
                }

            }
            catch (Exception ex)
            {

            }

            ////check if any role exists, if not create all roles.
            if (!_roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_User)).GetAwaiter().GetResult();

                //if roles not created, create admin user
                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "neville.rabakale@gmail.com",
                    Email = "neville.rabakale@gmail.com",
                    Name = "Neville Rabakale",

                }, MailPass).GetAwaiter().GetResult();

                //assign created user Role of User
                ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "neville.rabakale@gmail.com");
                _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();
            }
            return;

        }
    }
}
