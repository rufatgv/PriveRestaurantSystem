using Final.DAL;
using Final.Extensions;
using Final.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Final.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class VisionController : Controller
    {
        private readonly AppDbContext _context;
        public VisionController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(bool? status, int page = 1)
        {
            ViewBag.Status = status;

            IEnumerable<Vision> visions = await _context.Visions
                .Where(t => status != null ? t.IsDeleted == status : true)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            ViewBag.PageIndex = page;
            ViewBag.PageCount = Math.Ceiling((double)visions.Count() / 5);

            return View(visions.Skip((page - 1) * 5).Take(5));
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Vision vision)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }


            if (await _context.Visions.AnyAsync(t => t.VisionText.ToLower() == vision.VisionText.ToLower()))
            {
                ModelState.AddModelError("VisionText", "This Name already exists");
                return View();
            }

            vision.CreatedAt = DateTime.UtcNow.AddHours(4);

            await _context.Visions.AddAsync(vision);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Update(int? id, bool? status, int page = 1)
        {
            if (id == null) return BadRequest();

            Vision vision = await _context.Visions.FirstOrDefaultAsync(t => t.Id == id);

            if (vision == null) return NotFound();

            return View(vision);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Vision vision, bool? status, int page = 1)
        {
            if (!ModelState.IsValid) return View(vision);

            if (id == null) return BadRequest();

            if (id != vision.Id) return BadRequest();

            Vision dbVision = await _context.Visions.FirstOrDefaultAsync(t => t.Id == id);

            if (dbVision == null) return NotFound();



            if (vision.VisionText.CheckString())
            {
                ModelState.AddModelError("Name", "Name may can contain only letters");
                return View(vision);
            }

            if (await _context.Visions.AnyAsync(t => t.Id != vision.Id && t.VisionText.ToLower() == vision.VisionText.ToLower()))
            {
                ModelState.AddModelError("Name", "This Name already exists");
                return View(vision);
            }

            dbVision.VisionText = vision.VisionText;
            dbVision.UpdatedAt = DateTime.UtcNow.AddHours(4);
            await _context.SaveChangesAsync();
            return RedirectToAction("index", new { status, page });
        }
        public async Task<IActionResult> Delete(int? id, bool? status, int page = 1)
        {
            if (id == null) return BadRequest();
            Vision dbVision = await _context.Visions.FirstOrDefaultAsync(t => t.Id == id);
            if (dbVision == null) return NotFound();
            dbVision.IsDeleted = true;
            dbVision.DeletedAt = DateTime.UtcNow.AddHours(4);
            await _context.SaveChangesAsync();
            ViewBag.Status = status;
            IEnumerable<Vision> visions = await _context.Visions
                .Where(t => status != null ? t.IsDeleted == status : true)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            ViewBag.PageIndex = page;
            ViewBag.PageCount = Math.Ceiling((double)visions.Count() / 5);
            return RedirectToAction("index", new { status, page });
        }
        public async Task<IActionResult> Restore(int? id, bool? status, int page = 1)
        {
            if (id == null) return BadRequest();
            Vision dbVision = await _context.Visions.FirstOrDefaultAsync(t => t.Id == id);
            if (dbVision == null) return NotFound();
            dbVision.IsDeleted = false;
            dbVision.DeletedAt = null;
            await _context.SaveChangesAsync();
            ViewBag.Status = status;
            IEnumerable<Vision> visions = await _context.Visions
                .Where(t => status != null ? t.IsDeleted == status : true)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
            ViewBag.PageIndex = page;
            ViewBag.PageCount = Math.Ceiling((double)visions.Count() / 5);
            return RedirectToAction("index", new { status, page });
        }
    }
}
