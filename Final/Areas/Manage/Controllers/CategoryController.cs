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
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public CategoryController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        } 
        public async Task<IActionResult> Index(bool? status, int page = 1)
        {
            ViewBag.Status = status;


            IQueryable<Category> categories = _context.Categories
                .Include(t => t.Products)
                .Include(t=> t.Blogs)
                .OrderByDescending(t => t.CreatedAt);

            if (status != null)
                categories = categories.Where(c => c.IsDeleted == status);



            ViewBag.PageIndex = page;
            ViewBag.PageCount = Math.Ceiling((double)categories.Count() / 5);

            return View(await categories.Skip((page - 1) * 5).Take(5).ToListAsync());
        }

        public async Task<IActionResult> Create(bool? status, int page = 1)
        {
            ViewBag.MainCategory = await _context.Categories.Where(c => !c.IsDeleted).ToListAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category, bool? status, int page = 1)
        {
            ViewBag.MainCategory = await _context.Categories.Where(c => !c.IsDeleted).ToListAsync();

            if (!ModelState.IsValid)
            {
                return View();
            }

            if (string.IsNullOrWhiteSpace(category.Name))
            {
                ModelState.AddModelError("Name", "There should be no gaps");
                return View();
            }
            if (await _context.Categories.AnyAsync(t => t.Name.ToLower() == category.Name.ToLower()))
            {
                ModelState.AddModelError("Name", "This Name already exists");
                return View();
            }

            if (category.ImageFile != null)
            {
                if (!category.ImageFile.CheckFileContentType("image/jpeg"))
                {
                    ModelState.AddModelError("ImageFile", "The selected image type doesn't match");
                    return View();
                }

                if (!category.ImageFile.CheckFileSize(100000))
                {
                    ModelState.AddModelError("ImageFile", "The Size of the Selected Image Can Be Maximum 10000 Kb");
                    return View();
                }

                category.Image = category.ImageFile.CreateFile(_env, "assets", "img", "meals");
            }
            else
            {
                ModelState.AddModelError("ImageFile", "Image must be selected");
                return View();
            }
            category.CreatedAt = DateTime.UtcNow.AddHours(4);

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { status = status, page = page });
        }

        public async Task<IActionResult> Update(int? id, bool? status, int page = 1)
        {
            if (id == null) return BadRequest();

            Category category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

            if (category == null) return NotFound();

            ViewBag.MainCategory = await _context.Categories.Where(c => c.Id != id && !c.IsDeleted).ToListAsync();

            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Category category, bool? status, int page = 1)
        {
            ViewBag.MainCategory = await _context.Categories.Where(c => c.Id != id && !c.IsDeleted).ToListAsync();

            Category dbCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

            if (dbCategory == null) return NotFound();

            if (!ModelState.IsValid)
            {
                return View(dbCategory);
            }

            if (id != category.Id) return BadRequest();

            if (string.IsNullOrWhiteSpace(category.Name))
            {
                ModelState.AddModelError("Name", "Bosluq Olmamalidir");
                return View(dbCategory);
            }

            //tag.Name = tag.Name.Trim();



            if (await _context.Categories.AnyAsync(t => t.Id != id && t.Name.ToLower() == category.Name.ToLower()))
            {
                ModelState.AddModelError("Name", "Alreade Exists");
                return View(dbCategory);
            }

            if (category.ImageFile != null)
            {
                if (!category.ImageFile.CheckFileContentType("image/jpeg"))
                {
                    ModelState.AddModelError("ImageFile", "The selected image type doesn't match");
                    return View();
                }

                if (!category.ImageFile.CheckFileSize(100000))
                {
                    ModelState.AddModelError("ImageFile", "The Size of the Selected Image Can Be Maximum 10000 Kb");
                    return View();
                }

                //Helper.DeleteFile(_env, dbCategory.Image, "assets", "img", "meals");
                dbCategory.Image = category.ImageFile.CreateFile(_env, "assets", "img", "meals");

            }
            dbCategory.Name = category.Name;
            dbCategory.UpdatedAt = DateTime.UtcNow.AddHours(4);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { status = status, page = page });
        }

        public async Task<IActionResult> Delete(int? id, bool? status, int page = 1)
        {
            if (id == null) return BadRequest();

            Category dbCategory = await _context.Categories.FirstOrDefaultAsync(t => t.Id == id);

            if (dbCategory == null) return NotFound();

            dbCategory.IsDeleted = true;
            dbCategory.DeletedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();

            ViewBag.Status = status;


            IEnumerable<Category> categories = await _context.Categories
                .Include(t => t.Products)
                .Include(t=> t.Blogs)
                .Where(t => status != null ? t.IsDeleted == status : true)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            ViewBag.PageIndex = page;
            ViewBag.PageCount = Math.Ceiling((double)categories.Count() / 5);

            return PartialView("_CategoryIndexPartial", categories.Skip((page - 1) * 5).Take(5));
        }
        public async Task<IActionResult> Restore(int? id, bool? status, int page = 1)
        {
            if (id == null) return BadRequest();

            Category dbCategory = await _context.Categories.FirstOrDefaultAsync(t => t.Id == id);

            if (dbCategory == null) return NotFound();

            dbCategory.IsDeleted = false;

            await _context.SaveChangesAsync();

            ViewBag.Status = status;


            IEnumerable<Category> categories = await _context.Categories
                .Include(t => t.Products)
                 .Include(t => t.Blogs)
                .Where(t => status != null ? t.IsDeleted == status : true)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            ViewBag.PageIndex = page;
            ViewBag.PageCount = Math.Ceiling((double)categories.Count() / 5);

            return PartialView("_CategoryIndexPartial", categories.Skip((page - 1) * 5).Take(5));
        }
    }
}
