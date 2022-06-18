using HowManyCalories.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HowManyCalories.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        //Create dataTables
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Week> Weeks { get; set; }

    }
}