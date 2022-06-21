using HowManyCalories.Data;
using HowManyCalories.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HowManyCalories.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }


    }
}