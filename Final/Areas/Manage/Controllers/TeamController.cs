using Final.DAL;
using Final.Extensions;
using Final.Helpers;
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
    public class TeamController : Controller
    {
      
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public TeamController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index(bool? status, int page = 1)
        {
            ViewBag.Status = status;

            IEnumerable<Team> tags = await _context.Teams
                .Where(t => status != null ? t.IsDeleted == status : true)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            ViewBag.PageIndex = page;
            ViewBag.PageCount = Math.Ceiling((double)tags.Count() / 5);

            return View(tags.Skip((page - 1) * 5).Take(5));
        }
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Team team)
        {
            if (!ModelState.IsValid) return View();

            if (string.IsNullOrWhiteSpace(team.Name))
            {
                ModelState.AddModelError("Name", "Name is required");
                return View();
            }
            if (string.IsNullOrWhiteSpace(team.Position))
            {
                ModelState.AddModelError("Position", "Position name is required");
                return View();
            }
            if (team.ImageFile != null)
            {
                if (!team.ImageFile.CheckFileContentType("image/jpeg"))
                {
                    ModelState.AddModelError("ImageFile", "The selected image type doesn't match");
                    return View();
                }
                if (!team.ImageFile.CheckFileSize(10000))
                {
                    ModelState.AddModelError("ImageFile", "The Size of the Selected Image Can Be Maximum 10000 Kb");
                    return View();
                }
                team.Image = team.ImageFile.CreateFile(_env, "assets", "img", "chefs");
            }

            team.CreatedAt = DateTime.UtcNow.AddHours(4);

            await _context.Teams.AddAsync(team);
            await _context.SaveChangesAsync();

            return RedirectToAction("index");
        }

        public async Task<IActionResult> Update(int? id)
        {
            return View(await _context.Teams.FirstOrDefaultAsync(b => b.Id == id));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Team team, int page = 1)
        {

            if (id == null) return BadRequest();
            

            Team dbteam = await _context.Teams.FirstOrDefaultAsync(b => b.Id == id);
            if (!ModelState.IsValid)
            {
                return View(dbteam);
            }
            team.Image = dbteam.Image;

            if (dbteam == null) return NotFound();

            if (string.IsNullOrWhiteSpace(team.Name))
            {
                ModelState.AddModelError("Name", "Name is required");
                return View(dbteam);
            }
            if (string.IsNullOrWhiteSpace(team.Position))
            {
                ModelState.AddModelError("Position", "Position name is required");
                return View(dbteam);
            }
             dbteam.Image = team.Image;
            if (team.ImageFile != null)
            {
                if (!team.ImageFile.CheckFileContentType("image/jpeg"))
                {
                    ModelState.AddModelError("ImageFile", "The image type does not match");
                    return View();
                }
                if (!team.ImageFile.CheckFileSize(100000))
                {
                    ModelState.AddModelError("ImageFile", "The Size of the Selected Image Can Be Maximum 10000 Kb");
                    return View();

                }
               
                    //Helper.DeleteFile(_env, dbteam.Image, "assets", "img", "chefs");
              
                dbteam.Image = team.ImageFile.CreateFile(_env, "assets", "img", "chefs");
            }
            dbteam.Name = team.Name;
            dbteam.Position = team.Position;
            dbteam.UpdatedAt = DateTime.UtcNow.AddHours(4);
            await _context.SaveChangesAsync();
            return RedirectToAction("index", new { page = page });
        }
        public async Task<IActionResult> Delete(int? id, bool? status, int page = 1)
        {
            if (id == null) return BadRequest();

            Team dbteam = await _context.Teams.FirstOrDefaultAsync(t => t.Id == id);

            if (dbteam == null) return NotFound();

            dbteam.IsDeleted = true;
            dbteam.DeletedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();

            ViewBag.Status = status;

            IEnumerable<Team> teams = await _context.Teams

                .Where(t => status != null ? t.IsDeleted == status : true)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            ViewBag.PageIndex = page;
            ViewBag.PageCount = Math.Ceiling((double)teams.Count() / 5);



            return PartialView("_TeamIndexPartial", teams.Skip((page - 1) * 5).Take(5));
        }
        public async Task<IActionResult> Restore(int? id, bool? status, int page = 1)
        {
            if (id == null) return BadRequest();

            Team dbteam = await _context.Teams.FirstOrDefaultAsync(t => t.Id == id);

            if (dbteam == null) return NotFound();

            dbteam.IsDeleted = false;

            await _context.SaveChangesAsync();

            ViewBag.Status = status;

            IEnumerable<Team> teams = await _context.Teams

                .Where(t => status != null ? t.IsDeleted == status : true)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            ViewBag.PageIndex = page;
            ViewBag.PageCount = Math.Ceiling((double)teams.Count() / 5);



            return PartialView("_TeamIndexPartial", teams.Skip((page - 1) * 5).Take(5));
        }
    }
}
