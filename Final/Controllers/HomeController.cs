using Final.DAL;
using Final.Models;
using Final.ViewModels.Home;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Final.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            HomeVM homeVM = new HomeVM {
                Products = await _context.Products.Where(p => !p.IsDeleted).ToListAsync(),
                Teams = await _context.Teams.Where(p => !p.IsDeleted).ToListAsync(),
                HomeIntros = await _context.HomeIntros.Where(p => !p.IsDeleted).ToListAsync()
                //Tables = await _context.Tables.FirstOrDefaultAsync()

            };
            ViewBag.Category = await _context.Categories.Where(p=>p.Image != null).ToListAsync();
            return View(homeVM);
        }
        public async Task<IActionResult> CategoryFilter(int? id)
        {
            List<Product> product = await _context.Products.Where(p => p.CategoryId == id && !p.IsDeleted).Take(5).ToListAsync();

            return PartialView("_IndexCategoryPartial",product);
        }

        
    }
}
