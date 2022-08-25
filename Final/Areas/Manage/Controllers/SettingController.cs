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
    public class SettingController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public SettingController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.Settings.ToListAsync());
        }
        public async Task<IActionResult> Detail(int? id)
        {
            return View(await _context.Settings.FirstOrDefaultAsync());
        }
        public async Task<IActionResult> Update(int? id)
        {
            return View(await _context.Settings.FirstOrDefaultAsync());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Setting setting, bool? status, int page = 1)
        {
            if (id == null) return BadRequest();
            Setting dbSetting = await _context.Settings.FirstOrDefaultAsync(s => s.Id == id);
            if (dbSetting == null) return NotFound();

            if (!ModelState.IsValid) return View();

            if (setting.LogoImage != null)
            {
                if (!setting.LogoImage.CheckFileContentType("image/jpeg"))
                {
                    ModelState.AddModelError("ImageFile", "Secilen Seklin Novu Uygun");
                    return View();
                }

                if (!setting.LogoImage.CheckFileSize(300))
                {
                    ModelState.AddModelError("ImageFile", "Secilen Seklin Olcusu Maksimum 300 Kb Ola Biler");
                    return View();
                }
                Helper.DeleteFile(_env, dbSetting.Logo, "assets", "img", "logo");

                dbSetting.Logo = setting.LogoImage.CreateFile(_env, "assets", "img", "logo");
            }
            dbSetting.Phone = setting.Phone;
            dbSetting.WorkHours = setting.WorkHours;
            dbSetting.Address = setting.Address;
            dbSetting.Email = setting.Email;
            dbSetting.ContactUsTitle = setting.ContactUsTitle;
            dbSetting.ContactUsDescription = setting.ContactUsDescription;
            dbSetting.UpdatedAt = DateTime.UtcNow.AddHours(4);
            await _context.SaveChangesAsync();
            return RedirectToAction("index", new { status, page });
        }

    }
}
