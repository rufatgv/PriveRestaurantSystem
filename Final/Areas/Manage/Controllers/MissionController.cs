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
    public class MissionController : Controller
    {
        private readonly AppDbContext _context;
        public MissionController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(bool? status, int page = 1)
        {
            ViewBag.Status = status;

            IEnumerable<Mission> missions = await _context.Missions
                .Where(t => status != null ? t.IsDeleted == status : true)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            ViewBag.PageIndex = page;
            ViewBag.PageCount = Math.Ceiling((double)missions.Count() / 5);

            return View(missions.Skip((page - 1) * 5).Take(5));
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Mission mission)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
          

            if (await _context.Missions.AnyAsync(t => t.MissionText.ToLower() == mission.MissionText.ToLower()))
            {
                ModelState.AddModelError("MissionText", "This Name already exists");
                return View();
            }

            mission.CreatedAt = DateTime.UtcNow.AddHours(4);

            await _context.Missions.AddAsync(mission);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Update(int? id, bool? status, int page = 1)
        {
            if (id == null) return BadRequest();

            Mission mission = await _context.Missions.FirstOrDefaultAsync(t => t.Id == id);

            if (mission == null) return NotFound();

            return View(mission);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Mission mission, bool? status, int page = 1)
        {
            if (!ModelState.IsValid) return View(mission);

            if (id == null) return BadRequest();

            if (id != mission.Id) return BadRequest();

            Mission dbMission = await _context.Missions.FirstOrDefaultAsync(t => t.Id == id);

            if (dbMission == null) return NotFound();

          

            if (mission.MissionText.CheckString())
            {
                ModelState.AddModelError("Name", "Name may can contain only letters");
                return View(mission);
            }

            if (await _context.Missions.AnyAsync(t => t.Id != mission.Id && t.MissionText.ToLower() == mission.MissionText.ToLower()))
            {
                ModelState.AddModelError("Name", "This Name already exists");
                return View(mission);
            }

            dbMission.MissionText = mission.MissionText;
            dbMission.UpdatedAt = DateTime.UtcNow.AddHours(4);
            await _context.SaveChangesAsync();
            return RedirectToAction("index", new { status, page });
        }
        public async Task<IActionResult> Delete(int? id, bool? status, int page = 1)
        {
            if (id == null) return BadRequest();
            Mission dbMission = await _context.Missions.FirstOrDefaultAsync(t => t.Id == id);
            if (dbMission == null) return NotFound();
            dbMission.IsDeleted = true;
            dbMission.DeletedAt = DateTime.UtcNow.AddHours(4);
            await _context.SaveChangesAsync();
            ViewBag.Status = status;
            IEnumerable<Mission> missions = await _context.Missions
                .Where(t => status != null ? t.IsDeleted == status : true)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            ViewBag.PageIndex = page;
            ViewBag.PageCount = Math.Ceiling((double)missions.Count() / 5);
            return RedirectToAction("index", new { status, page });
        }
        public async Task<IActionResult> Restore(int? id, bool? status, int page = 1)
        {
            if (id == null) return BadRequest();
            Mission dbMission = await _context.Missions.FirstOrDefaultAsync(t => t.Id == id);
            if (dbMission == null) return NotFound();
            dbMission.IsDeleted = false;
            dbMission.DeletedAt = null;
            await _context.SaveChangesAsync();
            ViewBag.Status = status;
            IEnumerable<Mission> missions = await _context.Missions
                .Where(t => status != null ? t.IsDeleted == status : true)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
            ViewBag.PageIndex = page;
            ViewBag.PageCount = Math.Ceiling((double)missions.Count() / 5);
            return RedirectToAction("index", new { status, page });
        }
    }
}
