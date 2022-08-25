using Final.DAL;
using Final.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Final.Controllers
{
    public class MenuController : Controller
    {
        private readonly AppDbContext _context;
        public MenuController(AppDbContext context)
        {
            _context = context;
        }
        public async  Task<IActionResult> Index()
        {
            List<Product> products = await _context.Products.Where(p => !p.IsDeleted).ToListAsync();
            return View(products);
        }
    }
}
