using Final.DAL;
using Final.Extensions;
using Final.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Final.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class HomeIntroController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public HomeIntroController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.HomeIntros.ToListAsync());
        }
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Create(HomeIntro homeIntro)
        {
            if (!ModelState.IsValid) return View();
            if (await _context.HomeIntros.AnyAsync(t => t.Intro.ToLower() == homeIntro.Intro.ToLower()))
            {
                ModelState.AddModelError("Into", "This context already exsist");
                return View();
            }
            if (await _context.HomeIntros.AnyAsync(t => t.Title.ToLower() == homeIntro.Title.ToLower()))
            {
                ModelState.AddModelError("Title", "This title already exsist");
                return View();
            }
            if (await _context.HomeIntros.AnyAsync(t => t.Description.ToLower() == homeIntro.Description.ToLower()))
            {
                ModelState.AddModelError("Description", "This description already exsist");
                return View();
            }
            homeIntro.CreatedAt = DateTime.UtcNow.AddHours(4);
            await _context.HomeIntros.AddAsync(homeIntro);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Detail(int? id)
        {
            return View(await _context.HomeIntros.FirstOrDefaultAsync());
        }
        public async Task<IActionResult> Update(int? id)
        {
            return View(await _context.HomeIntros.FirstOrDefaultAsync());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, HomeIntro homeIntro, bool? status, int page = 1)
        {
            if (id == null) return BadRequest();
            HomeIntro dbhomeintro = await _context.HomeIntros.FirstOrDefaultAsync(s => s.Id == id);
            if (dbhomeintro == null) return NotFound();

            if (!ModelState.IsValid) return View();

          
           
            dbhomeintro.Intro = homeIntro.Intro;
            dbhomeintro.Title = homeIntro.Title;
            dbhomeintro.Description = homeIntro.Description;
            dbhomeintro.UpdatedAt = DateTime.UtcNow.AddHours(4);
            await _context.SaveChangesAsync();
            return RedirectToAction("index", new { status, page });
        }
        public async Task<IActionResult> Delete(int? id, bool? status, int page = 1)
        {
            if (id == null) return BadRequest();

            HomeIntro dbhomeIntro = await _context.HomeIntros.FirstOrDefaultAsync(t => t.Id == id);

            if (dbhomeIntro == null) return NotFound();

            dbhomeIntro.IsDeleted = true;
            dbhomeIntro.DeletedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();

            ViewBag.Status = status;

            IEnumerable<HomeIntro> homeIntros = await _context.HomeIntros

                .Where(t => status != null ? t.IsDeleted == status : true)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            ViewBag.PageIndex = page;
            ViewBag.PageCount = Math.Ceiling((double)homeIntros.Count() / 5);

            return RedirectToAction("index", new { status, page });


        }
        public async Task<IActionResult> Restore(int? id, bool? status, int page = 1)
        {
            if (id == null) return BadRequest();

            HomeIntro dbhomeIntro = await _context.HomeIntros.FirstOrDefaultAsync(t => t.Id == id);

            if (dbhomeIntro == null) return NotFound();

            dbhomeIntro.IsDeleted = false;

            await _context.SaveChangesAsync();

            ViewBag.Status = status;

            IEnumerable<HomeIntro> homeIntros = await _context.HomeIntros

                .Where(t => status != null ? t.IsDeleted == status : true)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            ViewBag.PageIndex = page;
            ViewBag.PageCount = Math.Ceiling((double)homeIntros.Count() / 5);

            return RedirectToAction("index", new { status, page });

        }
    }
}
