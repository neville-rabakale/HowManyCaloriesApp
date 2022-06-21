using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HowManyCalories.Data;
using HowManyCalories.Models;

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
            return View();
        }

    }
}
