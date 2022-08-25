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
    public class AboutIntroController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public AboutIntroController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.AboutIntros.ToListAsync());
        }

        public async Task<IActionResult> Create(bool? status, int page = 1)
        {
          
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AboutIntro aboutIntro)
        {
            if (!ModelState.IsValid) return View();
         
            
            if (await _context.AboutIntros.AnyAsync(t => t.Feature.ToLower() == aboutIntro.Feature.ToLower()))
            {
                ModelState.AddModelError("Description", "This description already exsist");
                return View();
            }
            if (aboutIntro.ImageFile != null)
            {
                if (!aboutIntro.ImageFile.CheckFileContentType("image/jpeg"))
                {
                    ModelState.AddModelError("ImageFile", "The selected image type doesn't match");
                    return View();
                }

                if (!aboutIntro.ImageFile.CheckFileSize(100000))
                {
                    ModelState.AddModelError("ImageFile", "The Size of the Selected Image Can Be Maximum 10000 Kb");
                    return View();
                }

                aboutIntro.Image = aboutIntro.ImageFile.CreateFile(_env, "assets", "img", "interier");
            }
            else
            {
                ModelState.AddModelError("ImageFile", "Image must be selected");
                return View();
            }
            aboutIntro.CreatedAt = DateTime.UtcNow.AddHours(4);
            await _context.AboutIntros.AddAsync(aboutIntro);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Detail(int? id)
        {
            return View(await _context.AboutIntros.FirstOrDefaultAsync());
        }
        public async Task<IActionResult> Update(int? id)
        {
            return View(await _context.AboutIntros.FirstOrDefaultAsync());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, AboutIntro aboutIntro, bool? status, int page = 1)
        {
            if (id == null) return BadRequest();
            AboutIntro dbaboutintro = await _context.AboutIntros.FirstOrDefaultAsync(s => s.Id == id);
            if (dbaboutintro == null) return NotFound();

            if (!ModelState.IsValid) return View();

            if (aboutIntro.ImageFile != null)
            {
                if (!aboutIntro.ImageFile.CheckFileContentType("image/jpeg"))
                {
                    ModelState.AddModelError("ImageFile", "The selected image type doesn't match");
                    return View();
                }

                if (!aboutIntro.ImageFile.CheckFileSize(100000))
                {
                    ModelState.AddModelError("ImageFile", "The Size of the Selected Image Can Be Maximum 10000 Kb");
                    return View();
                }

                dbaboutintro.Image = aboutIntro.ImageFile.CreateFile(_env, "assets", "img", "interier");

            }

            
            dbaboutintro.Feature = aboutIntro.Feature;
            dbaboutintro.ImageFile = aboutIntro.ImageFile;
            dbaboutintro.UpdatedAt = DateTime.UtcNow.AddHours(4);
            await _context.SaveChangesAsync();
            return RedirectToAction("index", new { status, page });
        }
        public async Task<IActionResult> Delete(int? id, bool? status, int page = 1)
        {
            if (id == null) return BadRequest();

            AboutIntro dbaboutIntro = await _context.AboutIntros.FirstOrDefaultAsync(t => t.Id == id);

            if (dbaboutIntro == null) return NotFound();

            dbaboutIntro.IsDeleted = true;
            dbaboutIntro.DeletedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();

            ViewBag.Status = status;

            IEnumerable<AboutIntro> aboutIntros = await _context.AboutIntros

                .Where(t => status != null ? t.IsDeleted == status : true)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            ViewBag.PageIndex = page;
            ViewBag.PageCount = Math.Ceiling((double)aboutIntros.Count() / 5);

            return RedirectToAction("index", new { status, page });


        }
        public async Task<IActionResult> Restore(int? id, bool? status, int page = 1)
        {
            if (id == null) return BadRequest();

            AboutIntro dbaboutIntro = await _context.AboutIntros.FirstOrDefaultAsync(t => t.Id == id);

            if (dbaboutIntro == null) return NotFound();

            dbaboutIntro.IsDeleted = false;

            await _context.SaveChangesAsync();

            ViewBag.Status = status;

            IEnumerable<AboutIntro> aboutIntros = await _context.AboutIntros

                .Where(t => status != null ? t.IsDeleted == status : true)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            ViewBag.PageIndex = page;
            ViewBag.PageCount = Math.Ceiling((double)aboutIntros.Count() / 5);

            return RedirectToAction("index", new { status, page });

        }
    }
}
