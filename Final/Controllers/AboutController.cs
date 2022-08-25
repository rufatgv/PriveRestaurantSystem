using Final.DAL;
using Final.Models;
using Final.ViewModels.About;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Final.Controllers
{
    public class AboutController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        public AboutController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            AboutVM aboutVM = new AboutVM
            {
                AboutIntros = await _context.AboutIntros.Where(p => !p.IsDeleted).ToListAsync(),
                Missions= await _context.Missions.Where(p=> !p.IsDeleted).ToListAsync(),
                Visions = await _context.Visions.Where(p=> !p.IsDeleted).ToListAsync()
                
            

            };
            ViewBag.Category = await _context.Categories.Where(p => p.Image != null).ToListAsync();
            return View(aboutVM);
        }
    }
}
 